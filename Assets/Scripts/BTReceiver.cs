using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TechTweaking.Bluetooth;

public class BTReceiver : MonoBehaviour {

	public Text connectionStateText;
	public string deviceName = "HC-05";
	public string deviceMACAddress = "XX:XX:XX:XX:XX:XX";
	public int numberOfBytes = 2;

	private DrumController drumController;
	private BluetoothDevice transmitter;
	//private bool isBTEnabled = false;
	//private bool deviceFound = false;

	void Awake() {
		transmitter = new BluetoothDevice ();

		if (BluetoothAdapter.isBluetoothEnabled ()) {
			connect ();
		} else {
			connectionStateText.text = "Please enable your Bluetooth.";

			BluetoothAdapter.OnBluetoothStateChanged += handleBTStateChange;
			BluetoothAdapter.listenToBluetoothState ();

			BluetoothAdapter.askEnableBluetooth ();
		}
	}

	void Start() {
		drumController = GetComponent<DrumController> ();
		BluetoothAdapter.OnDeviceOFF += handleDeviceOFF;
		BluetoothAdapter.OnDeviceNotFound += handleDeviceNotFound;
	}

	void connect() {
		connectionStateText.text = "Connecting...";

		transmitter.MacAddress = deviceMACAddress;
		transmitter.ReadingCoroutine = readFromTransmitter;

		transmitter.connect ();
	}

	void disconnect() {
		if (transmitter != null)
			transmitter.close ();
	}

	IEnumerator readFromTransmitter(BluetoothDevice device) {
		connectionStateText.text = "Connected - reading possible.";

		while (device.IsReading) {
			if (device.IsDataAvailable) {
				byte[] data = device.read ();

				if (data != null && data.Length >= numberOfBytes) {
					connectionStateText.text = data.Length + " bytes received: " + (int)(data [0]) + " and " + (int)(data [1]);
					drumController.Hit ((int) (data[0]), (int) (data[1]));
				}
			}

			yield return null;
		}

		connectionStateText.text = "Reading done.";
	}

	void handleBTStateChange(bool btEnabled) {
		if (btEnabled) {
			connect ();

			BluetoothAdapter.OnBluetoothStateChanged -= handleBTStateChange;
			BluetoothAdapter.stopListenToBluetoothState ();
		}
	}

	void handleDeviceOFF(BluetoothDevice device) {
		if (!string.IsNullOrEmpty (device.Name))
			connectionStateText.text = "Can\'t connect to device with name " + device.Name + " - device is OFF";
		else if (!string.IsNullOrEmpty(device.MacAddress))
			connectionStateText.text = "Can\'t connect to device with MAC " + device.MacAddress + " - device is OFF";
	}

	void handleDeviceNotFound(BluetoothDevice device) {
		if (!string.IsNullOrEmpty(device.Name))
			connectionStateText.text = "Can\'t find device with name " + device.Name + " - device may be OFF or not paired yet";
	}

	void OnDestroy() {
		disconnect ();
		BluetoothAdapter.OnDeviceOFF -= handleDeviceOFF;
		BluetoothAdapter.OnDeviceNotFound -= handleDeviceNotFound;
	}

	/* Old code */
	/*// Use this for initialization
	void Start () {
		drumController = GetComponent<DrumController> ();
		connectionStateText.text = "Initializing...";
		BluetoothAdapter.listenToBluetoothState ();

		if (!BluetoothAdapter.isBluetoothEnabled()) {
			connectionStateText.text = "BT disabled, asking...";
			BluetoothAdapter.OnBluetoothON += ScanForDevice;
			BluetoothAdapter.askEnableBluetooth ();
		} else
			ScanForDevice ();
	}

	public void ScanForDevice() {
		if (!isBTEnabled) {
			isBTEnabled = true;
			connectionStateText.text = "BT enabled, scanning...";
			BluetoothAdapter.OnDeviceDiscovered += (BluetoothDevice device, short rssi) => {
				if (!deviceFound && device.Name.Equals (DeviceName)) {
					deviceFound = true;
					connectionStateText.text = "Device found: " + device.Name + " with rssi " + rssi;
					ConnectToDevice (device);
				}
			};
			BluetoothAdapter.startDiscovery ();
		}
	}

	public void ConnectToDevice(BluetoothDevice device) {
		device.ReadingCoroutine = readBytesAndHitDrum;
		AddLoggingCallbacks (device);
		device.connect ();
	}

	private void AddLoggingCallbacks(BluetoothDevice device) {
		device.OnConnected += (obj) => connectionStateText.text = "Connected successfully.";
		device.OnDeviceNotFound += (obj) => connectionStateText.text = "Device not found";
		device.OnDeviceOFF += (obj) => connectionStateText.text = "Device found, but connect failed";
		device.OnDisconnected += (obj) => connectionStateText.text = "Disconnected successfully.";
		device.OnReadingError += (obj) => connectionStateText.text = "Read error";
		device.OnReadingStarted += (obj) => connectionStateText.text = "Read started.";
		device.OnReadingStoped += (obj) => connectionStateText.text = "Read stopped.";
	}

	public IEnumerator readBytesAndHitDrum(BluetoothDevice device) {
		while (device.IsReading) {
			if (device.IsDataAvailable) {
				byte[] data = device.read (numberOfBytes);

				if (data.Length >= numberOfBytes) {
					connectionStateText.text = data.Length + " bytes received: " + (int)(data [0]) + " and " + (int)(data [1]);
					drumController.Hit ((int) (data[0]), (int) (data[1]));
				}
			}
			yield return null;
		}
	}*/


}
