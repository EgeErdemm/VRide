using Android.BLE;
using Android.BLE.Commands;
using System; // BitConverter için
using System.Collections;
using System.Text; // Encoding için, Debug.Log'da kullanırsınız
using UnityEngine;
using UnityEngine.UI;
public class DeviceRowView : MonoBehaviour
{
    private Coroutine _connectRoutine;
    private string _deviceUuid = string.Empty;
    private string _deviceName = string.Empty;

    [SerializeField]
    private Text _deviceUuidText;
    [SerializeField]
    private Text _deviceNameText;

    [SerializeField]
    private Image _deviceButtonImage;
    [SerializeField]
    private Text _deviceButtonText;

    [SerializeField]
    private Color _onConnectedColor;

    [SerializeField]
    private Button _buttonComponent;
    private Color _previousColor;

    private bool _isConnected = false;

    private ConnectToDevice _connectCommand;
    // Plugin'deki sınıf adını kullanıyoruz: SubscribeToCharacteristic
    private SubscribeToCharacteristic _subscribeCharacteristicCommand;

    // Son okunan tekerlek dönüşlerini ve zamanını saklayacağız, hız hesaplamak için
    private uint _lastCumulativeWheelRevolutions = 0;
    private ushort _lastWheelEventTime = 0;
    private bool _firstCscReading = true; // İlk okumada hız hesaplamamak için

    // Son okunan krank dönüşlerini ve zamanını saklayacağız, kadans hesaplamak için
    private ushort _lastCumulativeCrankRevolutions = 0;
    private ushort _lastCrankEventTime = 0;

    // hiz degeri tuttuğum yer
    public float currentSpeed { get; private set; } = 0f;

    // panel referans veriyorum ki scale 0 yapayim connected oldugunda
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private RectTransform startGamePanel;


    private IEventBus eventBus;

    private void Awake()
    {
        eventBus =EventBus.Instance;
    }

    public void Show(string uuid, string name)
    {
        _deviceButtonText.text = "Connect";

        string deviceUuid = uuid;
        _deviceName = name;

        _deviceUuidText.text = uuid;
        _deviceNameText.text = name;

        if(_deviceName == "Wahoo SPEED 4578")
        {
            Debug.Log("Wahoo bulundu");
        }
        if (_deviceName != null && _deviceName.StartsWith("Wahoo"))
        {
            Debug.Log("Wahoo ile başlayan cihaz bulundu: " + _deviceName);
            // Burada bağlanmayı deneyebilirsin
            _connectCommand = new ConnectToDevice(deviceUuid, OnConnected, OnDisconnected);
            _deviceUuid = deviceUuid;
            BleManager.Instance.QueueCommand(_connectCommand);
        }

        _buttonComponent.onClick.AddListener(ToggleConnect);

        gameObject.SetActive(true);
    }

    public void ToggleConnect()
    {
        if (!_isConnected)
        {
            Debug.Log("connected device" + _deviceUuid +_deviceName);
            _connectCommand = new ConnectToDevice(_deviceUuid, OnConnected, OnDisconnected);
            BleManager.Instance.QueueCommand(_connectCommand);
        }
        else
        {
            _connectCommand.Disconnect();
            // Aboneliği iptal etme (plugin'in Unsubscribe metodu var)
            if (_subscribeCharacteristicCommand != null)
            {
                _subscribeCharacteristicCommand.Unsubscribe();
                _subscribeCharacteristicCommand = null;
            }
        }
        _buttonComponent.interactable = false;
    }

    // Yeni Subscribe Metodu
    public void SubscribeToCscMeasurement()
    {
        // Bluetooth SIG CSC Service UUID'si (uzun form)
        string cscServiceUuid = "00001816-0000-1000-8000-00805f9b34fb";
        // Bluetooth SIG CSC Measurement Characteristic UUID'si (uzun form)
        string cscMeasurementCharacteristicUuid = "00002a5b-0000-1000-8000-00805f9b34fb";

        // Plugin'deki SubscribeToCharacteristic yapısına uygun şekilde çağırıyoruz
        // customGatt parametresini true yapıyoruz çünkü uzun UUID'ler kullanıyoruz
        _subscribeCharacteristicCommand = new SubscribeToCharacteristic(
            _deviceUuid,
            cscServiceUuid,
            cscMeasurementCharacteristicUuid,
            OnCscMeasurementReceivedCallback, // Veri alındığında çağrılacak callback
            true // customGatt = true, çünkü uzun UUID'ler kullanıyoruz
        );
        BleManager.Instance.QueueCommand(_subscribeCharacteristicCommand);
        Debug.Log($"Subscribing to CSC Measurement on device {_deviceUuid} for service {cscServiceUuid} characteristic {cscMeasurementCharacteristicUuid} (CustomGatt: True)");
    }

    // CSC Measurement verileri geldiğinde çağrılacak metod (CharacteristicChanged delegate'ine uygun)
    private void OnCscMeasurementReceivedCallback(byte[] data)
    {
        // Debug.Log($"Raw data received: {BitConverter.ToString(data)}"); // Ham veriyi görmek için

        // Gelen bayt verilerini ayrıştır
        ParseCscMeasurementData(data);
    }

    private void ParseCscMeasurementData(byte[] data)
    {
        if (data == null || data.Length < 1)
        {
            Debug.LogWarning("Received empty or null CSC Measurement data.");
            return;
        }

        byte flags = data[0];
        bool wheelRevolutionDataPresent = (flags & 0x01) != 0; // Bit 0 set ise tekerlek verisi var
        bool crankRevolutionDataPresent = (flags & 0x02) != 0;  // Bit 1 set ise krank verisi var

        int offset = 1; // Flags'tan sonraki ofset

        //float currentSpeed = 0f; // km/h veya m/s
        float currentCadence = 0f; // RPM

        // Tekerlek Verisi İşleme (Wheel Revolutions)
        if (wheelRevolutionDataPresent && data.Length >= offset + 6)
        {
            // cumulative_wheel_revolutions: UINT32 (4 bayt)
            uint cumulativeWheelRevolutions = BitConverter.ToUInt32(data, offset);
            offset += 4;

            // last_wheel_event_time: UINT16 (2 bayt), 1/1024 saniye biriminde
            ushort lastWheelEventTime = BitConverter.ToUInt16(data, offset);
            offset += 2;

            if (!_firstCscReading)
            {
                // Farkları hesapla
                uint deltaRevolutions = cumulativeWheelRevolutions - _lastCumulativeWheelRevolutions;
                // Zaman farkı negatif olabilir (sensör zamanı sıfırlanırsa veya taşarsa), bu durumu ele almalıyız
                ushort deltaTime = 0;
                if (lastWheelEventTime >= _lastWheelEventTime)
                {
                    deltaTime = (ushort)(lastWheelEventTime - _lastWheelEventTime);
                }
                else
                {
                    // Zaman taşması (overflow) durumu, 65536 (2^16) ekleyerek düzeltilir
                    deltaTime = (ushort)(65536 - _lastWheelEventTime + lastWheelEventTime);
                }


                // Eğer zaman ilerlemişse ve tekerlek dönmüşse hız hesapla
                if (deltaTime > 0 && deltaRevolutions > 0)
                {
                    float timeInSeconds = deltaTime / 1024f; // Zamanı saniyeye çevir
                    float wheelCircumferenceMeters = 2.096f; // Örnek tekerlek çevresi (road bike 700x23c için yaklaşık, KENDİ TEKERLEK ÇEVRENİZİ GİRİN!)

                    // Hız (metre/saniye) = (dönüş sayısı farkı * tekerlek çevresi) / zaman farkı
                    currentSpeed = (deltaRevolutions * wheelCircumferenceMeters) / timeInSeconds;
                    Debug.Log($"Calculated Speed: {currentSpeed} m/s ({currentSpeed * 3.6f} km/h)");
                    eventBus.Publish(new SpeedEvent(currentSpeed));
                }
                else if (deltaRevolutions == 0 && deltaTime > 0)
                {
                    currentSpeed = 0f; // Tekerlek dönmediyse hız 0
                }
            }

            // Bir sonraki okuma için güncel değerleri sakla
            _lastCumulativeWheelRevolutions = cumulativeWheelRevolutions;
            _lastWheelEventTime = lastWheelEventTime;
        }

        // Krank Verisi İşleme (Crank Revolutions)
        if (crankRevolutionDataPresent && data.Length >= offset + 4)
        {
            // cumulative_crank_revolutions: UINT16 (2 bayt)
            ushort cumulativeCrankRevolutions = BitConverter.ToUInt16(data, offset);
            offset += 2;

            // last_crank_event_time: UINT16 (2 bayt), 1/1024 saniye biriminde
            ushort lastCrankEventTime = BitConverter.ToUInt16(data, offset);
            offset += 2;

            if (!_firstCscReading)
            {
                // Farkları hesapla
                ushort deltaCrankRevolutions = (ushort)(cumulativeCrankRevolutions - _lastCumulativeCrankRevolutions);
                // Zaman farkı negatif olabilir (sensör zamanı sıfırlanırsa veya taşarsa), bu durumu ele almalıyız
                ushort deltaCrankTime = 0;
                if (lastCrankEventTime >= _lastCrankEventTime)
                {
                    deltaCrankTime = (ushort)(lastCrankEventTime - _lastCrankEventTime);
                }
                else
                {
                    // Zaman taşması (overflow) durumu, 65536 (2^16) ekleyerek düzeltilir
                    deltaCrankTime = (ushort)(65536 - _lastCrankEventTime + lastCrankEventTime);
                }

                // Eğer zaman ilerlemişse ve krank dönmüşse kadans hesapla
                if (deltaCrankTime > 0 && deltaCrankRevolutions > 0)
                {
                    float timeInMinutes = (deltaCrankTime / 1024f) / 60f; // Zamanı dakikaya çevir
                    // Kadans (RPM) = dönüş sayısı farkı / zaman farkı (dakika)
                    currentCadence = deltaCrankRevolutions / timeInMinutes;
                    Debug.Log($"Calculated Cadence: {currentCadence} RPM");
                }
                else if (deltaCrankRevolutions == 0 && deltaCrankTime > 0)
                {
                    currentCadence = 0f; // Krank dönmediyse kadans 0
                }
            }

            // Bir sonraki okuma için güncel değerleri sakla
            _lastCumulativeCrankRevolutions = cumulativeCrankRevolutions;
            _lastCrankEventTime = lastCrankEventTime;
        }

        // İlk okuma tamamlandı, bundan sonra hız/kadans hesaplayabiliriz
        _firstCscReading = false;
        WhenConnected();

        // Burada UI elemanlarını güncelleyebilir veya hız/kadans değerlerini başka bir sisteme iletebilirsiniz.
    }

    private void OnConnected(string deviceUuid)
    {
        _previousColor = _deviceButtonImage.color;
        _deviceButtonImage.color = _onConnectedColor;

        _isConnected = true;
        _buttonComponent.interactable = true;
        _deviceButtonText.text = "Disconnect";

        // Bağlandıktan sonra CSC Measurement karakteristiğine abone ol
        SubscribeToCscMeasurement();
        if (_connectRoutine != null)
        {
            StopCoroutine(_connectRoutine);
            _connectRoutine = null;
        }
    }

    private void OnDisconnected(string deviceUuid)
    {
        _deviceButtonImage.color = _previousColor;
        _buttonComponent.interactable = true;
        _isConnected = false;
        _deviceButtonText.text = "Connect";

        // Bağlantı kesildiğinde ilk okuma bayrağını ve son okunan değerleri sıfırla
        _firstCscReading = true;
        _lastCumulativeWheelRevolutions = 0;
        _lastWheelEventTime = 0;
        _lastCumulativeCrankRevolutions = 0;
        _lastCrankEventTime = 0;


        // bağlantı kopunca tekrar dener
        if (_connectRoutine == null)
            _connectRoutine = StartCoroutine(TryConnectInLoop());
    }

    private void WhenConnected()
    {
        panelRectTransform.localScale = Vector3.zero;
        startGamePanel.localScale = Vector3.one;
    }


    IEnumerator TryConnectInLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            _connectCommand = new ConnectToDevice(_deviceUuid, OnConnected, OnDisconnected);
            BleManager.Instance.QueueCommand(_connectCommand);
        }
    }
}