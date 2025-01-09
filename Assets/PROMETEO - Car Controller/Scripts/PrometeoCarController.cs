using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    // --------------------------------------------------------------------------------------
    // 1) 자동차 설정(CAR SETUP) 관련 변수들
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90;
    // 자동차가 도달할 수 있는 최대 속도(km/h).

    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    // 자동차가 후진 시 도달할 수 있는 최대 속도(km/h).

    [Range(1, 10)]
    public int accelerationMultiplier = 2;
    // 가속도 배수(값이 높을수록 빠르게 가속).

    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27;
    // 운전대(핸들)를 돌릴 때 바퀴가 회전할 수 있는 최대 각도.

    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;
    // 바퀴가 회전하는 스피드(핸들 반응 속도).

    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350;
    // 바퀴에 적용되는 브레이크 힘의 크기.

    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    // 가속을 멈췄을 때 차량이 얼마나 빠르게 감속하는지 결정(값이 높을수록 빨리 감속).

    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;
    // 사이드브레이크(핸드브레이크) 사용 시, 차량이 얼마나 쉽게 미끄러지는지를 결정(값이 높을수록 더 크게 미끄러짐).

    [Space(10)]
    public Vector3 bodyMassCenter;
    /*
     * 차량의 질량 중심을 나타내는 벡터.
     * 일반적으로 x = 0, z = 0에 두고, y축만 적절히 조절합니다.
     * y값이 높아질수록 차량이 더 불안정해집니다.
    */

    // --------------------------------------------------------------------------------------
    // 2) 바퀴(WHEELS) 설정
    // --------------------------------------------------------------------------------------
    //[Header("WHEELS")]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;
    /*
     * 각 WheelCollider와 3D 메쉬가 분리된 게임 오브젝트여야 합니다.
     * 예) WheelCollider 오브젝트와, 그에 해당하는 3D 휠 모델(메쉬)는 다른 게임오브젝트.
    */

    // --------------------------------------------------------------------------------------
    // 3) 파티클 이펙트(PARTICLE SYSTEMS) 사용 여부 & 연결
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("EFFECTS")]
    [Space(10)]
    public bool useEffects = false;
    // 파티클 및 트레일 사용 여부

    // 차량이 드리프트할 때 사용되는 파티클 시스템(타이어 스모크 등)
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    // 차량이 미끄러질 때 사용되는 트레일 렌더러(스키드 마크)
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    // --------------------------------------------------------------------------------------
    // 4) UI - 속도 표시(SPEED TEXT) 관련
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("UI")]
    [Space(10)]
    public bool useUI = false;
    // UI를 사용할지 여부
    public Text carSpeedText;
    // 차량 속도를 표시할 UI 텍스트

    // --------------------------------------------------------------------------------------
    // 5) 사운드(SOUNDS) 관련
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("Sounds")]
    [Space(10)]
    public bool useSounds = false;
    // 차량 사운드 사용 여부
    public AudioSource carEngineSound;
    // 엔진 사운드
    public AudioSource tireScreechSound;
    // 타이어 스키드 사운드(드리프트 시)
    float initialCarEngineSoundPitch;
    // 엔진 사운드 기본 피치 값을 저장하기 위함

    // --------------------------------------------------------------------------------------
    // 6) 컨트롤(CONTROLS) - 모바일용 터치 컨트롤 여부
    // --------------------------------------------------------------------------------------
    [Space(20)]
    //[Header("CONTROLS")]
    [Space(10)]
    public bool useTouchControls = false;
    // 터치 컨트롤 사용 여부

    public GameObject throttleButton;
    PrometeoTouchInput throttlePTI;
    public GameObject reverseButton;
    PrometeoTouchInput reversePTI;
    public GameObject turnRightButton;
    PrometeoTouchInput turnRightPTI;
    public GameObject turnLeftButton;
    PrometeoTouchInput turnLeftPTI;
    public GameObject handbrakeButton;
    PrometeoTouchInput handbrakePTI;

    // --------------------------------------------------------------------------------------
    // 7) 차량 정보(CAR DATA)
    // --------------------------------------------------------------------------------------
    [HideInInspector]
    public float carSpeed;
    // 현재 차량 속도(계산 용도)

    [HideInInspector]
    public bool isDrifting;
    // 차량이 드리프트 중인지 여부

    [HideInInspector]
    public bool isTractionLocked;
    // 핸드브레이크로 인해 접지력이 고정(타이어가 잠긴) 상태인지 여부

    // --------------------------------------------------------------------------------------
    // 8) 내부에서만 사용하는 변수들(PRIVATE VARIABLES)
    // --------------------------------------------------------------------------------------
    Rigidbody carRigidbody;
    // 차량의 Rigidbody
    float steeringAxis;
    // 스티어링(핸들) 축(-1 ~ 1)
    float throttleAxis;
    // 가속 관련 축(-1 ~ 1)
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool touchControlsSetup = false;

    // 바퀴의 마찰 관련 커브. 드리프트나 트랙션에 사용
    WheelFrictionCurve FLwheelFriction;
    float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction;
    float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction;
    float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction;
    float RRWextremumSlip;

    // --------------------------------------------------------------------------------------
    // Start() 메서드
    // 게임 시작 후 첫 프레임에 한 번만 호출되며, 전체 세팅을 초기화/설정
    // --------------------------------------------------------------------------------------
    void Start()
    {
        // (1) Rigidbody를 가져와서, 인스펙터에서 설정한 질량 중심으로 지정
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        // (2) 드리프트용 마찰 수치 초기화
        FLwheelFriction = new WheelFrictionCurve();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;

        FRwheelFriction = new WheelFrictionCurve();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;

        RLwheelFriction = new WheelFrictionCurve();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;

        RRwheelFriction = new WheelFrictionCurve();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // (3) 엔진 사운드 기본 피치값 저장
        if (carEngineSound != null)
        {
            initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        // (4) 주행 속도 UI와 사운드 업데이트 메서드를 주기적으로 호출
        if (useUI)
        {
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        }
        else if (!useUI)
        {
            if (carSpeedText != null)
            {
                carSpeedText.text = "0";
            }
        }

        if (useSounds)
        {
            InvokeRepeating("CarSounds", 0f, 0.1f);
        }
        else if (!useSounds)
        {
            if (carEngineSound != null)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null)
            {
                tireScreechSound.Stop();
            }
        }

        // (5) 이펙트 사용 여부 처리
        if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }

        // (6) 터치 컨트롤 사용 시, 각 버튼 연결 확인 및 PrometeoTouchInput 가져오기
        if (useTouchControls)
        {
            if (throttleButton != null && reverseButton != null &&
                turnRightButton != null && turnLeftButton != null &&
                handbrakeButton != null)
            {
                throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                touchControlsSetup = true;
            }
            else
            {
                String ex = "터치 컨트롤이 올바르게 설정되지 않았습니다. PrometeoCarController 컴포넌트에서 " +
                            "버튼들을 제대로 드래그 앤 드롭해야 합니다.";
                Debug.LogWarning(ex);
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // Update() 메서드
    // 매 프레임마다 호출되며, 사용자 입력과 차량 물리를 처리
    // --------------------------------------------------------------------------------------
    void Update()
    {
        // (1) 차량 속도 측정
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        // (2) 지역 좌표계에서의 x, z축 속도 계산
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        // (3) 차량 컨트롤 로직 - 터치 컨트롤 vs 키보드(PC)
        if (useTouchControls && touchControlsSetup)
        {
            // 전진
            if (throttlePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            // 후진
            if (reversePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            // 좌회전
            if (turnLeftPTI.buttonPressed)
            {
                TurnLeft();
            }
            // 우회전
            if (turnRightPTI.buttonPressed)
            {
                TurnRight();
            }
            // 핸드브레이크
            if (handbrakePTI.buttonPressed)
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (!handbrakePTI.buttonPressed)
            {
                RecoverTraction();
            }

            // 가속X, 후진X -> 속도 감소
            if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
            {
                ThrottleOff();
            }
            if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }

            // 스티어링 축 초기화
            if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
        }
        else
        {
            // PC 키보드 입력
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                GoReverse();
            }
            if (Input.GetKey(KeyCode.A))
            {
                TurnLeft();
            }
            if (Input.GetKey(KeyCode.D))
            {
                TurnRight();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke("DecelerateCar");
                deceleratingCar = false;
                Handbrake();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                RecoverTraction();
            }

            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
            {
                ThrottleOff();
            }
            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
        }

        // (4) 바퀴 콜라이더와 실제 바퀴(3D 메쉬)의 위치/회전 동기화
        AnimateWheelMeshes();
    }

    // --------------------------------------------------------------------------------------
    // CarSpeedUI()
    // UI 텍스트에 차량 속도를 표시하기 위한 메서드
    // --------------------------------------------------------------------------------------
    public void CarSpeedUI()
    {
        if (useUI)
        {
            try
            {
                float absoluteCarSpeed = Mathf.Abs(carSpeed);
                carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // CarSounds()
    // 차량 사운드를 제어하는 메서드 (엔진음 피치, 드리프트 사운드 등)
    // --------------------------------------------------------------------------------------
    public void CarSounds()
    {
        if (useSounds)
        {
            try
            {
                // 엔진 사운드 피치 = 기본 피치 + 속도 기반 증가
                if (carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
                    carEngineSound.pitch = engineSoundPitch;
                }
                // 드리프트 사운드: 미끄러지거나(드리프트) 접지력이 잠겼는데(핸드브레이크) 속도가 어느정도 있을 때
                if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Play();
                    }
                }
                else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                {
                    tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            // 사운드 사용 안 함
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // TurnLeft()
    // 왼쪽으로 핸들을 돌림
    // --------------------------------------------------------------------------------------
    public void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // TurnRight()
    // 오른쪽으로 핸들을 돌림
    // --------------------------------------------------------------------------------------
    public void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // ResetSteeringAngle()
    // 핸들을 중앙으로 되돌림
    // --------------------------------------------------------------------------------------
    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // --------------------------------------------------------------------------------------
    // AnimateWheelMeshes()
    // WheelCollider 정보를 바탕으로 3D 휠 메쉬의 위치 및 회전을 동기화
    // --------------------------------------------------------------------------------------
    void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    // --------------------------------------------------------------------------------------
    // GoForward()
    // 전진을 위한 양의 토크(엔진 힘) 적용
    // --------------------------------------------------------------------------------------
    public void GoForward()
    {
        // x축 방향으로 힘이 크게 걸리면 드리프트 상태로 인식
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // 가속도 축을 부드럽게 1까지 증가
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        // 만약 차량이 후진 중이면 -> 브레이크 걸어야 함
        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            // 최대 속도 이하일 때만 토크를 준다
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // 최대 속도에 도달하면 토크를 0으로
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // GoReverse()
    // 후진을 위한 음의 토크(엔진 힘) 적용
    // --------------------------------------------------------------------------------------
    public void GoReverse()
    {
        // x축 방향으로 힘이 크게 걸리면 드리프트 상태로 인식
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // 가속도 축을 부드럽게 -1까지 감소
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }

        // 전진 중이면 -> 브레이크
        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            // 최대 후진 속도 이하일 때만 토크 적용
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // 최대 후진 속도에 도달하면 토크 0
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // ThrottleOff()
    // 가속을 멈춤(토크 0)
    // --------------------------------------------------------------------------------------
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    // --------------------------------------------------------------------------------------
    // DecelerateCar()
    // 차량 감속 로직 (InvokeRepeating으로 주기적으로 불려서 차량을 서서히 감속)
    // --------------------------------------------------------------------------------------
    public void DecelerateCar()
    {
        // x축 방향으로 힘이 크게 걸리면 드리프트 상태로 인식
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // throttleAxis를 부드럽게 0으로 복귀
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }

        // Rigidbody 속도를 decelerationMultiplier에 비례해서 줄임
        carRigidbody.linearVelocity = carRigidbody.linearVelocity * (1f / (1f + (0.025f * decelerationMultiplier)));

        // 바퀴 토크 제거
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;

        // 아주 느린 속도(0.25f 이하)면 완전히 정지
        if (carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    // --------------------------------------------------------------------------------------
    // Brakes()
    // 바퀴에 브레이크 토크 적용
    // --------------------------------------------------------------------------------------
    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    // --------------------------------------------------------------------------------------
    // Handbrake()
    // 사이드브레이크(핸드브레이크)를 사용하여 접지력을 약화 -> 드리프트 유도
    // --------------------------------------------------------------------------------------
    public void Handbrake()
    {
        CancelInvoke("RecoverTraction");

        // driftingAxis를 0에서 1까지 서서히 증가시켜 최대로 미끄러지는 상태를 만듦
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }

        // x축 힘이 크면 드리프트 중
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        // 미끄러짐 증가
        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;
        }

        // 핸드브레이크 사용 중 -> 트랙션 잠김
        isTractionLocked = true;
        DriftCarPS();
    }

    // --------------------------------------------------------------------------------------
    // DriftCarPS()
    // 차량이 드리프트할 때 파티클/트레일 효과를 제어
    // --------------------------------------------------------------------------------------
    public void DriftCarPS()
    {
        if (useEffects)
        {
            try
            {
                if (isDrifting)
                {
                    RLWParticleSystem.Play();
                    RRWParticleSystem.Play();
                }
                else if (!isDrifting)
                {
                    RLWParticleSystem.Stop();
                    RRWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                {
                    RLWTireSkid.emitting = true;
                    RRWTireSkid.emitting = true;
                }
                else
                {
                    RLWTireSkid.emitting = false;
                    RRWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }
    }

    // --------------------------------------------------------------------------------------
    // RecoverTraction()
    // 핸드브레이크 해제 시, 다시 정상 접지력으로 되돌리는 함수
    // --------------------------------------------------------------------------------------
    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }

        // 원래의 마찰값으로 되돌아올 때까지 서서히 복원
        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);
        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }
}
