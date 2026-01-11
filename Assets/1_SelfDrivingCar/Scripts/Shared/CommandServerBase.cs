using UnityEngine;
using System;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;

public abstract class CommandServerBase : MonoBehaviour
{
    protected SocketIOComponent socket;
    public Camera FrontFacingCamera;
    // protected CarController carController;

    protected void InitSocket()
    {
        if (socket != null) return;
        var go = GameObject.Find("SocketIO");
        if (go == null)
        {
            Debug.LogError("SocketIO GameObject not found in scene.");
            return;
        }
        socket = go.GetComponent<SocketIOComponent>();
    }

    protected void RegisterHandler(string evt, Action<SocketIOEvent> handler)
    {
        if (socket == null) InitSocket();
        if (socket != null) socket.On(evt, handler);
    }

    protected void Enqueue(Action action)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try { action?.Invoke(); }
            catch (Exception ex) { Debug.LogError($"CommandServerBase action error: {ex}"); }
        });
    }

    protected void EmitTelemetrySafe(JSONObject payload)
    {
        if (socket == null) return;
        socket.Emit("telemetry", payload);
    }

    protected float ConvertUnityYawToMathAngle(float psi)
    {
        if (psi >= 0 && psi <= 90) return 90 - psi;
        else if (psi > 90 && psi <= 180) return 90 + 270 - (psi - 90);
        else if (psi > 180 && psi <= 270) return 180 + 90 - (psi - 180);
        return 270 - 90 - (psi - 270);
    }

    // Shared helpers
    protected bool IsManualInputActive()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
    }

    protected string CaptureFrameBase64(Camera cam)
    {
        if (cam == null) return "";
        return Convert.ToBase64String(CameraHelper.CaptureFrame(cam));
    }

    protected bool TryGetFloat(JSONObject data, string field, out float value)
    {
        value = 0f;
        if (data == null || !data.HasField(field)) return false;
        try
        {
            value = data.GetField(field).f;
            return true;
        }
        catch
        {
            return float.TryParse(data.GetField(field).ToString(), out value);
        }
    }

    protected abstract void EmitTelemetry(SocketIOEvent obj);
}
