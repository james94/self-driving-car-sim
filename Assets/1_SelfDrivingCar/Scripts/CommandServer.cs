using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System.Security.AccessControl;
using System.Globalization;

public class CommandServer : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;
	private SocketIOComponent _socket;
	private CarController _carController;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);
		_carController = CarRemoteControl.GetComponent<CarController>();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		EmitTelemetry(obj);
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Manually");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Autonomously via PID Steering");

		Debug.Log("PID Control Active");

		try {
			// Add null check
			if(obj.data == null) {
				Debug.LogError("Empty steering command received");
				return;
			}

			// Add parse error handling
			JSONObject jsonObject = obj.data;

			Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");

			if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
				Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
				return;
			}

			// Add format verification
			float steeringAngle = jsonObject.GetField("steering_angle").f;
			float throttle = jsonObject.GetField("throttle").f;

			Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
			Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");

			CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
			CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);

			EmitTelemetry(obj);
		} catch(Exception ex) {
			Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
		}
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			try {

				print("Attempting to Send...");
				// send only if it's not being manually driven
				if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
					_socket.Emit("telemetry", new JSONObject());
				}
				else {
					// Add CTE calculation (replace with your actual CTE logic)
					float currentCTE = CalculateCTE();

					// Collect Simulated Data from the Car
					JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Add fields individually
					telemetryData.AddField("cte", currentCTE);
					telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle);
					telemetryData.AddField("throttle", _carController.AccelInput);
					telemetryData.AddField("speed", _carController.CurrentSpeed);
					telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
					_socket.Emit("telemetry", telemetryData);
				}

			} catch(Exception ex) {
				Debug.LogError($"Telemetry error: {ex}");
			}


		});
	}

	// Add this temporary CTE calculation method
	float CalculateCTE()
	{
		// Replace with your actual CTE calculation logic
		return UnityEngine.Random.Range(0.0f, 1.0f);
	}
}