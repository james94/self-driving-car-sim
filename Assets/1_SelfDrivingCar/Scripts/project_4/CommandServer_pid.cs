using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;
using UnityEngine.SceneManagement;

public class CommandServer_pid : CommandServerBase
{
	public CarRemoteControlTerm2 CarRemoteControl;
	// public Camera FrontFacingCamera;
	private SocketIOComponent _socket;
	private CarControllerTerm2 _carController;
	private WaypointTracker_pid wpt;

	// Use this for initialization
	void Start()
	{
		InitSocket();
		RegisterHandler("open", OnOpen);
		RegisterHandler("reset", OnReset);
		RegisterHandler("steer", OnSteer);
		RegisterHandler("manual", onManual);
		_carController = CarRemoteControl.GetComponent<CarControllerTerm2>();
		wpt = new WaypointTracker_pid ();
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
        Debug.Log("Manual driving event ...");
		EmitTelemetry (obj);
	}

	void OnReset(SocketIOEvent obj)
	{
		SceneManager.LoadScene("LakeTrackAutonomous_pid");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
        Debug.Log("Steering data event ...");
		JSONObject jsonObject = obj.data;

		float steering, throttle;
		bool sOk = TryGetFloat(jsonObject, "steering_angle", out steering);
		bool tOk = TryGetFloat(jsonObject, "throttle", out throttle);
		if (!sOk || !tOk)
		{
			Debug.LogError("Invalid steer/throttle payload");
			return;
		}

		CarRemoteControl.SteeringAngle = steering;
		CarRemoteControl.Acceleration = throttle;

		var steering_bias = 1.0f * Mathf.Deg2Rad;
		CarRemoteControl.SteeringAngle += steering_bias;

		EmitTelemetry(obj);
	}

	protected override void EmitTelemetry(SocketIOEvent obj)
	{
		Enqueue(() =>
		{
			try
			{
				// send only if it's not being manually driven
				if (IsManualInputActive())
				{
					socket.Emit("telemetry", new JSONObject());
				}
				else
				{
					var telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Cross-track error and car signals
					float cte = wpt.CrossTrackError(_carController);
					telemetryData.AddField("cte", cte);
					telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle);
					telemetryData.AddField("throttle", _carController.AccelInput);
					telemetryData.AddField("speed", _carController.CurrentSpeed);

					// Camera image (base64)
					telemetryData.AddField("image", CaptureFrameBase64(FrontFacingCamera));
					// telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));

					socket.Emit("telemetry", telemetryData);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Telemetry error: {ex}");
			}
		});
	}
}