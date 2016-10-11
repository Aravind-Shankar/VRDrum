using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BTLEReceiver : MonoBehaviour {

	public Text connectionStateText;

	private DrumController drumController;

	public string DeviceName = "glovemodule";
	public string ServiceUUID = "FFE0";
	public string SubscribeCharacteristic = "FFE1";
	public string ConfigDescriptor = "2902";

	private byte[] ENABLE_NOTIFICATION_VALUE = null;

	enum States
	{
		None,
		Scan,
		ScanRSSI,
		Connect,
		Subscribe,
		Receiving,
		Unsubscribe,
		Disconnect,
	}

	private bool _connected = false;
	private float _timeout = 0f;
	private States _state = States.None;
	private string _deviceAddress;
	private bool _foundSubscribeID = false;
	private bool _foundWriteID = false;
	private byte[] _dataBytes = null;
	//private bool _rssiOnly = false;
	//private int _rssi = 0;

	void Reset ()
	{
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_deviceAddress = null;
		_foundSubscribeID = false;
		//_foundWriteID = false;
		_dataBytes = null;
		//_rssi = 0;

		ENABLE_NOTIFICATION_VALUE = new byte[2];
		ENABLE_NOTIFICATION_VALUE [0] = 0x01;
		ENABLE_NOTIFICATION_VALUE [1] = 0x00;
	}

	void SetState (States newState, float timeout)
	{
		_state = newState;
		_timeout = timeout;
	}

	void StartProcess ()
	{
		Reset ();
		BluetoothLEHardwareInterface.Initialize (true, false, () => {

			SetState (States.Scan, 0.1f);

		}, (error) => {

			//BluetoothLEHardwareInterface.Log ("Error during initialize: " + error);
			connectionStateText.text = "Error during initialize: " + error;
		});
	}

	// Use this for initialization
	void Start () {
		drumController = GetComponent<DrumController> ();
		StartProcess ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = 0f;

				switch (_state)
				{
				case States.None:
					connectionStateText.text = "N/A";
					break;

				case States.Scan:
					connectionStateText.text = "Scanning";
					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

						// if your device does not advertise the rssi and manufacturer specific data
						// then you must use this callback because the next callback only gets called
						// if you have manufacturer specific data

						//if (!_rssiOnly)
						//{
						connectionStateText.text = "Callback 1 called; name = " + name;
							if (name.Contains (DeviceName))
							{
								connectionStateText.text = "name contained. address = " + address;
								BluetoothLEHardwareInterface.StopScan ();

								// found a device with the name we want
								// this example does not deal with finding more than one
								_deviceAddress = address;
								SetState (States.Connect, 0.5f);
							}
						//}

					}, (address, name, rssi, bytes) => {

						// use this one if the device responses with manufacturer specific data and the rssi

						connectionStateText.text = "Callback 2 called; name = " + name;
						if (name.Contains (DeviceName))
						{
							connectionStateText.text = "name contained. address = " + address;
							BluetoothLEHardwareInterface.StopScan ();

							// found a device with the name we want
							// this example does not deal with finding more than one
							_deviceAddress = address;
							SetState (States.Connect, 0.5f);
						}

						/*if (name.Contains (DeviceName))
						{
							if (_rssiOnly)
							{
								_rssi = rssi;
							}
							else
							{
								BluetoothLEHardwareInterface.StopScan ();

								// found a device with the name we want
								// this example does not deal with finding more than one
								_deviceAddress = address;
								SetState (States.Connect, 0.5f);
							}
						}*/

					}/*, _rssiOnly*/); // this last setting allows RFduino to send RSSI without having manufacturer data

					//if (_rssiOnly)
					//	SetState (States.ScanRSSI, 0.5f);
					break;

				case States.ScanRSSI:
					break;

				case States.Connect:
					connectionStateText.text = "Connecting; address = " + _deviceAddress;
					// set these flags
					_foundSubscribeID = false;
					//_foundWriteID = false;

					// note that the first parameter is the address, not the name. I have not fixed this because
					// of backwards compatiblity.
					// also note that I am not using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
					// the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
					// large enough that it will be finished enumerating before you try to subscribe or do any other operations.
					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) => {

						if (IsEqual (serviceUUID, ServiceUUID))
						{
							_foundSubscribeID = _foundSubscribeID || IsEqual (characteristicUUID, SubscribeCharacteristic);
							//_foundWriteID = _foundWriteID || IsEqual (characteristicUUID, WriteCharacteristic);

							// if we have found both characteristics that we are waiting for
							// set the state. make sure there is enough timeout that if the
							// device is still enumerating other characteristics it finishes
							// before we try to subscribe
							if (_foundSubscribeID /*&& _foundWriteID*/ )
							{
								_connected = true;
								SetState (States.Subscribe, 2f);
							}
						}
					});
					break;

				case States.Subscribe:
					connectionStateText.text = "Subscribing to module";
					BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddress, ServiceUUID, SubscribeCharacteristic,
						(deviceAddress, characteristicUUID) => {
							if (IsEqual(characteristicUUID, SubscribeCharacteristic)) {
								connectionStateText.text = "notif with matching uuid";
								BluetoothLEHardwareInterface.WriteCharacteristic(DeviceName, ServiceUUID, ConfigDescriptor,
									ENABLE_NOTIFICATION_VALUE, ENABLE_NOTIFICATION_VALUE.Length, true, (message) => {
										connectionStateText.text = "Write finished, message = " + message;
									});
								BluetoothLEHardwareInterface.ReadCharacteristic(DeviceName, ServiceUUID, characteristicUUID,
									(cUUID, bytes) => {
										_state = States.Receiving;

										// we received some data from the device
										_dataBytes = bytes;
									});
							}
						}, (address, characteristicUUID, bytes) => {
							_state = States.Receiving;

							// we received some data from the device
							_dataBytes = bytes;
					});
					break;
				
				case States.Receiving:
					connectionStateText.text = "Received bytes: " + _dataBytes[0] + "\t" + _dataBytes[1];
					drumController.Hit ( (int)(_dataBytes[0]), (int)(_dataBytes[1]) );
					break;

				case States.Unsubscribe:
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_deviceAddress, ServiceUUID, SubscribeCharacteristic, null);
					SetState (States.Disconnect, 4f);
					break;

				case States.Disconnect:
					if (_connected)
					{
						BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddress, (address) => {
							BluetoothLEHardwareInterface.DeInitialize (() => {

								_connected = false;
								_state = States.None;
							});
						});
					}
					else
					{
						BluetoothLEHardwareInterface.DeInitialize (() => {

							_state = States.None;
						});
					}
					break;
				}
			}
		}
	}

	string FullUUID (string uuid)
	{
		return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
	}

	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);

		return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
	}
}
