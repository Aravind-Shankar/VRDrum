using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TechTweaking.Bluetooth;

public class BTReceiver : MonoBehaviour {

	public Text connectionStateText;
	public string DeviceName = "HC-05";
	public int numberOfBytes = 2;

	private DrumController drumController;
	private bool isBTEnabled = false;
	private bool deviceFound = false;

	// Use this for initialization
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

	public void ConnectToDevice(BluetoothDevice transmitter) {
		transmitter.ReadingCoroutine = readBytesAndHitDrum;
		AddLoggingCallbacks (transmitter);
		transmitter.connect ();
	}

	private void AddLoggingCallbacks(BluetoothDevice transmitter) {
		transmitter.OnConnected += (obj) => connectionStateText.text = "Connected successfully.";
		transmitter.OnDeviceNotFound += (obj) => connectionStateText.text = "Device not found";
		transmitter.OnDeviceOFF += (obj) => connectionStateText.text = "Device found, but connect failed";
		transmitter.OnDisconnected += (obj) => connectionStateText.text = "Disconnected successfully.";
		transmitter.OnReadingError += (obj) => connectionStateText.text = "Read error";
		transmitter.OnReadingStarted += (obj) => connectionStateText.text = "Read started.";
		transmitter.OnReadingStoped += (obj) => connectionStateText.text = "Read stopped.";
	}

	public IEnumerator readBytesAndHitDrum(BluetoothDevice transmitter) {
		while (transmitter.IsReading) {
			if (transmitter.IsDataAvailable) {
				byte[] data = transmitter.read (numberOfBytes);

				if (data.Length >= numberOfBytes) {
					connectionStateText.text = data.Length + " bytes received: " + (int)(data [0]) + " and " + (int)(data [1]);
					drumController.Hit ((int) (data[0]), (int) (data[1]));
				}
			}
			yield return null;
		}
	}
}
