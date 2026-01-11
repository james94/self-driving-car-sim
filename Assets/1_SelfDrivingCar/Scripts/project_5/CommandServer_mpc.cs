using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;

public class CommandServer_mpc : CommandServerBase
{
	public CarRemoteControlTerm2 CarRemoteControl;
	// public Camera FrontFacingCamera;
	private CarControllerTerm2 _carController;
	private PointTracker point_path;
	private WaypointTracker_mpc wpt;
	private int polyOrder;

	// Use this for initialization
	void Start()
	{
		InitSocket();
		RegisterHandler("open", OnOpen);
		RegisterHandler("steer", OnSteer);
		RegisterHandler("manual", onManual);
		_carController = CarRemoteControl.GetComponent<CarControllerTerm2>();
		point_path = CarRemoteControl.GetComponent<PointTracker>();
		wpt = new WaypointTracker_mpc ();
		polyOrder = 5;
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

	void onManual(SocketIOEvent obj)
	{
        Debug.Log("Manual driving event ...");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
        Debug.Log("Steering data event ...");
		JSONObject jsonObject = obj.data;

		float steering, throttle;
		if (!TryGetFloat(jsonObject, "steering_angle", out steering) ||
		    !TryGetFloat(jsonObject, "throttle", out throttle))
		{
			Debug.LogError("Invalid steer/throttle payload");
			return;
		}

		CarRemoteControl.SteeringAngle = steering;
		CarRemoteControl.Acceleration = throttle;

		// Next points
		var next_x = jsonObject.GetField("next_x");
		var next_y = jsonObject.GetField("next_y");
		List<float> my_next_x = new List<float>();
		List<float> my_next_y = new List<float>();
		for (int i = 0; i < next_x.Count; i++)
		{
			my_next_x.Add(float.Parse(next_x[i].ToString()));
			my_next_y.Add(float.Parse(next_y[i].ToString()));
		}
		point_path.setNextPoint(my_next_x, my_next_y);

		// MPC points
		var mpc_x = jsonObject.GetField("mpc_x");
		var mpc_y = jsonObject.GetField("mpc_y");
		List<float> my_mpc_x = new List<float>();
		List<float> my_mpc_y = new List<float>();
		for (int i = 0; i < mpc_x.Count; i++)
		{
			my_mpc_x.Add(float.Parse(mpc_x[i].ToString()));
			my_mpc_y.Add(float.Parse(mpc_y[i].ToString()));
		}
		point_path.setMpcPoint(my_mpc_x, my_mpc_y);

		EmitTelemetry(obj);
	}

	protected override void EmitTelemetry(SocketIOEvent obj)
	{
		Enqueue(() =>
		{
			try
			{
				if (IsManualInputActive())
				{
					socket.Emit("telemetry", new JSONObject());
				}
				else
				{
					var telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Waypoints arrays
					JSONObject ptsx = new JSONObject(JSONObject.Type.ARRAY);
					JSONObject ptsy = new JSONObject(JSONObject.Type.ARRAY);
					for (int i = wpt.prev_wp; i < wpt.prev_wp + polyOrder + 1; i++)
					{
						int idx = i % wpt.waypoints.Count;
						ptsx.Add(wpt.waypoints[idx].x);
						ptsy.Add(wpt.waypoints[idx].z);
					}
					telemetryData.AddField("ptsx", ptsx);
					telemetryData.AddField("ptsy", ptsy);

					// Orientation
					var psiDeg = _carController.Orientation().eulerAngles.y;
					telemetryData.AddField("psi_unity", psiDeg * Mathf.Deg2Rad);
					telemetryData.AddField("psi", ConvertUnityYawToMathAngle(psiDeg) * Mathf.Deg2Rad);

					// Position
					var pos = _carController.Position();
					telemetryData.AddField("x", pos.x);
					telemetryData.AddField("y", pos.z);

					// Signals
					telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle * Mathf.Deg2Rad);
					telemetryData.AddField("throttle", _carController.AccelInput);
					telemetryData.AddField("speed", _carController.CurrentSpeed);

					// CTE
					float cte = wpt.CrossTrackError(_carController);
					telemetryData.AddField("cte", cte);

					// Optional: image capture if needed for MPC
					// telemetryData.AddField("image", CaptureFrameBase64(FrontFacingCamera));

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