using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TechTweaking.Bluetooth;

public class BtDiscovery : MonoBehaviour
{

	Dictionary<string,DeviceData> MacAddressToBluetoothDevice;
	class DeviceData
	{
		public BluetoothDevice device;
		public short RSSI;
		
		public DeviceData (BluetoothDevice dev, short RSSI)
		{
			this.device = dev;
			this.RSSI = RSSI;
		}
	}

	public Text DevicesText;//ScrollTerminalUI is a script used to control the ScrollView text


	// Use this for initialization
	void Start ()
	{
		BluetoothAdapter.askEnableBluetooth ();

		MacAddressToBluetoothDevice = new Dictionary<string,DeviceData> ();
		BluetoothAdapter.OnDeviceDiscovered += HandleOnDeviceDiscovered;

	}

	void HandleOnDeviceDiscovered (BluetoothDevice dev, short rssi)
	{
		//Remember : dev != any other devices even if they're targeting the same device(because they're a connection container for the targeted device). 
		//to check if they target the same device check there mac address
		if (!MacAddressToBluetoothDevice.ContainsKey (dev.MacAddress)) {
			MacAddressToBluetoothDevice.Add (dev.MacAddress, new DeviceData (dev, rssi));
		} else {
			DeviceData devData = MacAddressToBluetoothDevice [dev.MacAddress];
			devData.RSSI = rssi;
		}


		DevicesText.text = "";
		foreach (KeyValuePair<string, DeviceData> entry in MacAddressToBluetoothDevice) {
			DeviceData inst = entry.Value;
			DevicesText.text += inst.device.Name + '\n' + inst.device.MacAddress + '\n' + "RSSI : " + inst.RSSI;
			DevicesText.text += "---------" + '\n';
		}
	}

	public void startDiscovery ()
	{
		BluetoothAdapter.startDiscovery ();
	}

	void OnDestroy ()
	{
		BluetoothAdapter.OnDeviceDiscovered -= HandleOnDeviceDiscovered;
	}

}