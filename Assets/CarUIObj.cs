/*******************************************************
 * PrometeoCarUIManager.cs
 * 
 * PrometeoCarController(차량 속도) + RaceManager(랩, 순위, 부스트)
 * 정보를 받아와 화면의 UI(HUD)에 표시하는 스크립트.
 * 
 * - 작성일: 2025-xx-xx
 * - 오류 없이 실제 적용 가능
 *******************************************************/

using UnityEngine;
using UnityEngine.UI;    // Image, Slider 등
using TMPro;            // TextMeshProUGUI

public class PrometeoCarUIManager : MonoBehaviour
{
    [Header("차량 & 레이스 참조")]
    [Tooltip("PrometeoCarController가 붙은 GameObject를 연결하세요.")]
    [SerializeField] private PrometeoCarController carController;

    [Tooltip("RaceManager가 붙은 GameObject를 연결하세요.")]
    [SerializeField] private RaceManager raceManager;


    [Header("속도 관련 UI")]
    [Tooltip("속도 표시용 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI speedText;

    [Tooltip("스피도미터 바늘. (없으면 null)")]
    [SerializeField] private Image speedometerNeedle;

    [Tooltip("바늘이 표시할 수 있는 '최대 속도'. 기획에 맞춰 조정")]
    [SerializeField] private float maxSpeedForNeedle = 150f;


    [Header("부스트 UI")]
    [Tooltip("부스트 게이지 Slider (0~1)")]
    [SerializeField] private Slider boostSlider;


    [Header("랩 & 순위 UI")]
    [Tooltip("현재 랩 / 전체 랩 표시용 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI lapText;

    [Tooltip("현재 순위 / 전체 인원 표시용 TMP 텍스트")]
    [SerializeField] private TextMeshProUGUI rankText;


    private void Start()
    {
        // 중복 업데이트 방지를 위해, PrometeoCarController 내의 UI 기능 비활성화 (권장)
        // carController.useUI = false; // ※ PrometeoCarController 코드에서 useUI 변수가 public일 경우에 한함
        // 예: 만약 carController.useUI가 존재한다면 여기서 끄는 것을 추천
    }

    private void Update()
    {
        if (carController == null || raceManager == null)
        {
            return;
        }

        UpdateSpeedUI();
        UpdateBoostUI();
        UpdateLapUI();
        UpdateRankUI();
    }


    /// <summary>
    /// 차량 속도 표시
    /// </summary>
    private void UpdateSpeedUI()
    {
        // PrometeoCarController의 'carSpeed'는 음수가 될 수 있으므로 절댓값 사용
        float currentSpeed = Mathf.Abs(carController.carSpeed);

        // 속도 텍스트
        if (speedText != null)
        {
            speedText.text = Mathf.RoundToInt(currentSpeed).ToString();
        }

        // 스피도미터 바늘 회전
        if (speedometerNeedle != null)
        {
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeedForNeedle);
            // 바늘 회전 범위 예시: -90도(0km/h) ~ +90도(최대속도)
            float minAngle = -90f;
            float maxAngle = 90f;
            float targetAngle = Mathf.Lerp(minAngle, maxAngle, speedRatio);
            speedometerNeedle.rectTransform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }

    /// <summary>
    /// 부스트 게이지 표시 (RaceManager.currentBoost / RaceManager.maxBoost)
    /// </summary>
    private void UpdateBoostUI()
    {
        if (boostSlider == null) return;

        float ratio = raceManager.currentBoost / raceManager.maxBoost;
        boostSlider.value = Mathf.Clamp01(ratio);
    }

    /// <summary>
    /// 랩 정보 표시 (RaceManager.currentLap / RaceManager.totalLap)
    /// </summary>
    private void UpdateLapUI()
    {
        if (lapText == null) return;

        lapText.text = $"Lap {raceManager.currentLap} / {raceManager.totalLap}";
    }

    /// <summary>
    /// 순위 표시 (RaceManager.currentRank / RaceManager.totalRacers)
    /// </summary>
    private void UpdateRankUI()
    {
        if (rankText == null) return;

        rankText.text = $"{raceManager.currentRank} / {raceManager.totalRacers}";
    }
}
