/*******************************************************
 * RaceManager.cs
 * 
 * 레이스 상태(현재 랩/총 랩, 현재 순위/총 인원, 부스트 등)를
 * 관리하는 스크립트.
 * 
 * - 작성일: 2025-xx-xx
 * - 오류 없이 실제 적용 가능
 *******************************************************/

using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("랩 & 순위 설정")]
    [Tooltip("현재 플레이어(또는 차량)가 몇 번째 랩인지")]
    public int currentLap = 1;

    [Tooltip("총 몇 랩으로 구성된 레이스인지")]
    public int totalLap = 3;

    [Tooltip("현재 몇 위인지")]
    public int currentRank = 1;

    [Tooltip("총 몇 명(대수)이 레이스에 참여하는지")]
    public int totalRacers = 8;

    [Header("부스트(니트로) 설정")]
    [Tooltip("현재 남아 있는 부스트 게이지")]
    public float currentBoost = 0f;

    [Tooltip("최대 부스트 게이지")]
    public float maxBoost = 100f;

    [Tooltip("부스트 사용 시 초당 소모량")]
    public float boostConsumeRate = 30f;

    [Tooltip("주행 중(예: 가속, 드리프트 등)에 부스트가 조금씩 차오르는 양")]
    public float boostRechargeRate = 5f;


    private void Update()
    {
        // 실제 게임 상황에 맞춰 부스트를 충전/소모하는 로직을 예시로 작성
        // 필요에 따라 다른 곳에서 키 입력을 받아 UseBoost() 등을 호출해도 됩니다.
        RechargeBoostOverTime();
    }

    /// <summary>
    /// 랩을 하나 늘리는 함수 (체크포인트 콜라이더 등에서 호출)
    /// </summary>
    public void IncreaseLap()
    {
        if (currentLap < totalLap)
        {
            currentLap++;
        }
        else
        {
            // 이미 완주했거나, 추가 처리(레이스 종료) 할 수도 있음
            Debug.Log("레이스가 완료되었습니다!");
        }
    }

    /// <summary>
    /// 순위를 갱신하는 함수 (게임 매니저에서 플레이어 위치 계산 후 호출)
    /// </summary>
    /// <param name="newRank">새로운 순위</param>
    public void UpdateRank(int newRank)
    {
        currentRank = Mathf.Clamp(newRank, 1, totalRacers);
    }

    /// <summary>
    /// 부스트 사용 (버튼 누를 때 등에서 호출)
    /// </summary>
    /// <param name="deltaTime">프레임 간 시간 (초단위)</param>
    public void UseBoost(float deltaTime)
    {
        float amount = boostConsumeRate * deltaTime;
        currentBoost = Mathf.Max(0f, currentBoost - amount);
    }

    /// <summary>
    /// 자동으로 부스트가 충전되는 로직 (이 예시는 계속 충전되는 형태)
    /// 실제 게임에서는 드리프트 중, 가속 중 등 조건에 따라 부스트가 차오르도록 조정 가능
    /// </summary>
    private void RechargeBoostOverTime()
    {
        if (currentBoost < maxBoost)
        {
            currentBoost += boostRechargeRate * Time.deltaTime;
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }
        }
    }
}
