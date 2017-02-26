using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

using TechTweaking.Bluetooth;

public class BTReceiver : MonoBehaviour {

	[System.Serializable]
	public class ReadBytesEvent : UnityEvent<int, int> {}

	[System.Serializable]
	public class BTConnectedEvent : UnityEvent {}

	public Text connectionStateText;
	public string deviceName = "HC-05";
	public string deviceMACAddress = "XX:XX:XX:XX:XX:XX";
	public int numberOfBytes = 2;
	public BTConnectedEvent OnBTModuleConnected;
	public ReadBytesEvent OnReadBytes;

	private BluetoothDevice transmitter;

	void Start() {
		transmitter = new BluetoothDevice ();

		if (BluetoothAdapter.isBluetoothEnabled ()) {
			connect ();
		} else {
			connectionStateText.text = "Please enable your Bluetooth.";

			BluetoothAdapter.OnBluetoothStateChanged += handleBTStateChange;
			BluetoothAdapter.listenToBluetoothState ();

			BluetoothAdapter.askEnableBluetooth ();
		}

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
		OnBTModuleConnected.Invoke();

		while (device.IsReading) {
			if (device.IsDataAvailable) {
				byte[] data = device.read ();

				if (data != null && data.Length >= numberOfBytes) {
					//connectionStateText.text = data.Length + " bytes received: " + (int)(data [0]) + " and " + (int)(data [1]);
					connectionStateText.text = "Data is being received successfully.";
					OnReadBytes.Invoke ((int) (data[0]), (int) (data[1]));
				}
			}

			yield return null;
		}

		//connectionStateText.text = "Reading done.";
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
}
