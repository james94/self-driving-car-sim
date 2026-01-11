using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// NOTE: Term2 and Term3 inherited from MonoSingleton<UISystem> previously
// Does our parent need to as well?
public abstract class UISystemBase : MonoBehaviour
// public class UISystemBase : MonoSingleton<UISystem>
{
    // Common MPH UI across terms
    public Text MPH_Text;
    public Image MPH_Animation;

    protected float topSpeed;

    protected void InitTopSpeed(float top)
    {
        topSpeed = Mathf.Max(0.01f, top);
    }

    protected void SetMPHValue(float value)
    {
        if (MPH_Text != null) MPH_Text.text = value.ToString("N2");
        if (MPH_Animation != null && topSpeed > 0f) MPH_Animation.fillAmount = value / topSpeed;
    }

    // Shared Escape handling (override per term if needed)
    protected virtual void OnEscapePressed()
    {
        SceneManager.LoadScene("MenuScene");
    }

    protected void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed();
        }
    }
}
