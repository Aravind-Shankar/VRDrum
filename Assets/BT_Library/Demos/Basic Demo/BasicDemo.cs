using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using TechTweaking.Bluetooth;

public class BasicDemo : MonoBehaviour {

	private  BluetoothDevice device;
	public Text statusText;
	// Use this for initialization
	void Awake () {
		
		BluetoothAdapter.enableBluetooth();//Force Enabling Bluetooth


		device = new BluetoothDevice();

		/*
		 * We need to identefy the device either by its MAC Adress or Name (NOT BOTH! it will use only one of them to identefy your device).
		 *
		 *---------- MacAdress property
		 * Using the MAC adress is the best choice because the device doesn't have to be paired!
		 * 
		 * ----------Name property
		 * Using the 'Name' property makes the Plugin search through your Paired devices list, so you must pair your remote device!.
		 * 
		 * If you want to use the 'Name' property without having the device previously paired, you need to start a heavy discovery process using one of the followings : 
		 * 		1) call BluetoothAdapter.startDiscovery(), and give it enough time to add all the nearby devices as 'fake' paired devices.
		 * 		2) You can  do this by adjusting the parmaeters of the connect method (check the docs!),
		 * 		but that will block any further connection until the device discovery has finished which might take 12 seconds or your device has been found.
		 */


		device.Name = "HC-05";
		//device.MacAddress = "XX:XX:XX:XX:XX:XX";



		/*
		 * 10 equals the char '\n' which is a "new Line" in Ascci representation, 
		 * so the read() method will retun a packet that was ended by the byte 10. simply read() will read lines.
		 */
		device.setEndByte (10);


		/*
		 * The ManageConnection Coroutine will start when the device is ready for reading.
		 */
		device.ReadingCoroutine = ManageConnection;
		
		
	}
	
	public void connect() {
		statusText.text = "Status : ...";

		/*
		 * Notice that there're more than one connect() method, check out the docs to read about them.
		 * a simple device.connect() is equivalent to connect(10, 1000, false) which will make 10 connection attempts
		 * before failing completly, each attempt will cost at least 1 second.
		 * -----------
		 * To alter that  check out the following methods in the docs :
		 * connect (int attempts, int time, bool allowDiscovery) 
		 * normal_connect (bool isBrutal, bool isSecure)
		 */
		device.connect();

	}

	public void disconnect() {
		device.close();
	}


	//############### Reading Data  #####################
	//Please note that you don't have to use this Couroutienes/IEnumerator, you can just put your code in the Update() method
	IEnumerator  ManageConnection (BluetoothDevice device)
	{
		statusText.text = "Status : Connected & Can read";
		
		while (device.IsReading) {
			if (device.IsDataAvailable) {
				byte [] msg = device.read ();//because we called setEndByte(10)..read will always return a packet excluding the last byte 10.
				
				if (msg != null && msg.Length > 0) {
					string content = System.Text.ASCIIEncoding.ASCII.GetString (msg);
					statusText.text = "MSG : " + content;
				}
			}
			
			yield return null;
		}
		
		statusText.text = "Status : Done Reading";
	}

}
