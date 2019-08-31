using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_MainObject : MonoBehaviour
{
    /* 
     * SerializeField - private은 유지되면서 인스펙터 창에 표시되는 변수 값 변경이 가능
     * * 단 모든 변수가 인스펙터 창에 표시되는 것은 아님
     */
        // 스피드 조정 변수들
    [SerializeField]
    /* 캐릭터의 이동 속도 */
    private float walkSpeed;
    /* 캐릭터가 이동하고 있는지 유무 */
    private bool isWalk;
    [SerializeField]
    /* 캐릭터의 달리기 속도 */
    private float runSpeed;
    /* 실제 플레이어가 이동하는 속도 */
    private float applySpeed;
    /* 달리는지 유무를 나타내는 변수 */
    private bool isRun;

        // 캐릭터 점프 변수들
    [SerializeField]
    /* 캐릭터의 점프 정도 */
    private float jumpForce;
    /* 땅에 붙어있는지 유무 */
    private bool isGround;

        // 캐릭터 앉기 변수들
    [SerializeField]
    /* 캐릭터의 앉은 후 Y 값 */
    private float CrouchPosY;
    [SerializeField]
    /* 캐릭터가 앉는 속도 */
    private float crouchSpeed;
    /* 캐릭터의 앉기 전 Y 값 */
    private float originPosY;
    /* 실제 케릭터에게 적용된 Y 값 */
    private float applyCrouchPosY;
    /* 앉아 있는지 유무 */
    private bool isCrouch = false;

        // 마우스 입력에 의한 변수들
    [SerializeField]
    /* 캐릭터의 마우스 민감도 */
    private float lookSensitivity;
    [SerializeField]
    /* 캐릭터 화면 회전 제한 (너무 많은 각도로 회전하지 않게 하기 위함) */
    private float cameraRotationLimit;
    /* 캐릭터 화면 가로 축의 현재 회전 각도 */
    private float currentCameraRotationX = 0.0f;

    /* 
     * 캐릭터의 화면( 카메라 ) 객체 
     * 여러 개의 카메라 중, 특정 카메라만 선택하기 위해 인스펙터 창에서 직접 넣어줄 것임
     */
        // 컴포넌트, 게임 오브젝트 변수들
    [SerializeField]
    private Camera theCamera;
    /* 캐릭터의 물리적 몸체 - 충돌 영역 */
    private Rigidbody myRigid;
    /* 캐릭터의 충돌 영역 */
    private CapsuleCollider CharacterCollider;
    private GunController theGunController;
    /* HUD의 크로스 헤어 객체 */
    private CrossHair theCrossHair;

    /* 플레이어의 이전 마지막 위치 변수 */
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        /* Script가 넣어진 오브젝트의 컴포넌트들 중, Temprate에 해당하는 컴포넌트들을 적용 */
        CharacterCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrossHair = FindObjectOfType<CrossHair>();

        /* 각 변수들의 초기화 */
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // 매 프레임( 초당 60 프레임 )마다 실행되는 함수
    void Update()
    {
        /* 캐릭터가 지면에 있는지 공중에 있는지 파악 */
        SetIsGround();
        /* 키 입력에 따른 캐릭터 점프 시도 */
        TryJump();
        /* 앉은 상태와 서 있는 상태 변경 시도 */
        TryCrouch();
        /* 키 입력에 따른 캐릭터 달리기 시도 */
        TryRun();
        /* 캐릭터 전 후 좌 우 이동 */
        Move();
        /* 이동 여부 파악 */
        MoveCheck();
        /* 마우스 입력에 따른 플레이어 화면(카메라) 상 하 이동 */
        CameraRotation();
        /* 마우스 입력에 따른 캐릭터, 카메라 좌 우 회전 */
        CharacterRotation();
    }

    /* 전후 좌우 캐릭터 이동 */
    private void Move()
    {
        /* 
         * Unity에 명시되어 있는 Horizontal 키
         * 키에 따른 수평 방향 값을 반환
         */
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        /*
         * Unity에 명시되어 있는 Vertical 키
         * 키에 따른 수직 방향 값을 반환
         */
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        /* transform.right = (1, 0, 0) */
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        /* transform.forward = (0, 0, 1) */
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        /*
         * 수평 입력 값과 수직 입력 값을 더함
         * 연산의 단순화를 위해 더한 Vector3를 표준화 함
         * 표준화된 값에 캐릭터 속도를 곱함
         * => 캐릭터의 이동 속도
         */
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        /* 
         * Time.deltaTime -> 1 프레임의 대략적인 시간
         * 1초( 60 프레임 )동안 _velocity 만큼의 위치를 이동하게 됨
         */
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void MoveCheck()
    {
        /* 달리기, 앉기, 점프한 상태를 제외하고 확인 */
        if(!isRun && !isCrouch && isGround)
        {
            /* 위치 변동이 있을 경우 걷는 상태(뛰거나) */
            /*
             * Vector3.Distance 함수 : 매개변수간의 거리를 측정
             * 조금만 차이가 나도 움직이는 것으로 판단할 수 있기 때문에 약간의 오차를 둠
             * 이유는 모르겠지만 이전 조건문에선 isWalk 값이 계속해서 변하는 문제가 있음
             */
            //if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
            if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
                isWalk = true;
            /* 위치 변동이 없을 경우 가만히 있는 상태 */
            else
                isWalk = false;
            /* 크로스 헤어에 상태 적용  */
            theCrossHair.WalkingAnimation(isWalk);
            /* 마지막 위치 변경 */
            lastPos = transform.position;
        }
    }

    /* 상하 카메라 이동 */
    private void CameraRotation()
    {
        /* 마우스의 상 하 방향 변수 값 */
        float _XRotation = Input.GetAxisRaw("Mouse Y");
        /* 마우스의 민감 정도를 적용해 카메라 회전 정도를 정의 */
        float _cameraRotationX = _XRotation * lookSensitivity;
        /*
         * 카메라의 상 하 방향 입력에 대해 반전 시켜줌
         * 실제 마우스 이동대로 카메라 시선이 이동할 것임
         */
        currentCameraRotationX -= _cameraRotationX;
        /* 
         * currentCameraRotationX 변수 값을 특정 영역에 고정 
         * ( 고정할 값 , 최소 고정 값, 최대 고정 값 ) 
         */
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        /* 
         * 계산이 완료된 회전 값을 카메라 객체에 적용
         * transform.localEulerAngles => 해당 오브젝트의 회전 값
         */
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
    }

    private void CharacterRotation()
    {
        /* 마우스의 좌 우 방향 변수 값 */
        float _yRotation = Input.GetAxisRaw("Mouse X");
        /* 마우스 민감 정도를 적용 */
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        float _charRotationY = _yRotation * lookSensitivity;
        /* 
         * 해당 오브젝트의 Rigidbody 회전에 좌 우 방향을 적용
         * Unity는 내부적으로 회전에 Quaternion을 사용함
         * Vector3 값을 Quaternion으로 변형
         */
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    // 달리는 것을 시도하는 변수
    private void TryRun()
    {
        /* 달리는 키를 눌렀을 경우, 달리기 시작 */
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        /* 달리는 키를 땟을 경우, 달리기 종료 */
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopRunning();
        }
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    // 앉기를 시도하는 함수
    private void TryCrouch()
    {
        /* 키 입력을 통해 앉은 상태와 서 있는 상태를 변경 */
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // 달리기를 적용하는 함수
    private void Running()
    {
        /* 앉은 상태에서 달리려 할 경우, 앉은 상태게 해제됨 */
        if(isCrouch)
            Crouch();

        /* 정조준 상태일 경우, 정조준 상태를 해제함 */
        theGunController.CancelFineSight();

        /* 달리기 상태를 True로 변경 */
        isRun = true;
        /* 크로스헤어에 상태 적용 */
        theCrossHair.RunningAnimation(isRun);
        /* 적용된 캐릭터 속도를 달리기로 변경 */
        applySpeed = runSpeed;
    }

    // 달리기를 중지하는 함수
    private void StopRunning()
    {
        isRun = false;
        /* 크로스 헤어에 상태 적용  */
        theCrossHair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    // 캐릭터가 점프하는 함수
    private void Jump()
    {
        /* 점프할 때 캐릭터가 앉아 있을 경우 서 있는 상태로 변경 */
        if (isCrouch)
            Crouch();
        /* 
         * Rigidbody.velocity : 해당 물체의 이동하는 속도
         * trasnform.up : (0, 1, 0)
         */
        myRigid.velocity = transform.up * jumpForce;
        isGround = false;
    }

    private void SetIsGround()
    {
        /*
         * Physics.Rascast
         *      주어진 특정 위치에서 주어진 다른 위치로 레이저를 쏨
         *      해당 물체가 떠 있는지 떠 있지 않은지 알기 위함
         * Collider의 bounds -> 충돌 영역(물체의 물리적 영역)의 범위
         * bounds의 extents -> 그 범위의 반
         * => 즉 캡슐의 반만큼 반사해 지면과 닿아 있는지 닿아 있지 않은지 파악 가능
         * 하지만 정확 거리를 세울 경우, 대각선에 위치해 있을 때, 닿아 있지 않다고 측정할 수 있음
         */
        isGround = Physics.Raycast(transform.position, Vector3.down, CharacterCollider.bounds.extents.y+0.1f);
        /* 점프할 때, 달리는 것과 같이 크로스 헤어를 변경해주기 위함 */
        theCrossHair.RunningAnimation(!isGround);
    }

    // 앉은 상태, 서 있는 상태 변경 함수
    private void Crouch()
    {
        /* 앉은 상태의 변형 */
        isCrouch = !isCrouch;
        /* 크로스 헤어에 상태 적용  */
        theCrossHair.CrouchingAnimation(isCrouch);
        Debug.Log(isCrouch);
        /* 앉아 있을 경우, 앉은 속도와 카메라 위치 변경 */
        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = CrouchPosY;
        }
        /* 서 잇을 경우, 서 있는 속도(걷는 속도)와 카메라 위치 변경 */
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        /* 
         * 카메라의 지역 위치에서 Y 축 위치만 변경함 
         * -> 자연스러운 카메라 이동을 위해 코루틴 함수로 변경
         */
        StartCoroutine(CrouchCoroutine());
    }

    // 앉을 때 부드러운 카메라 처리를 위한 코루틴 함수
    private IEnumerator CrouchCoroutine()
    {
        int cnt = 0;
        float _posY = theCamera.transform.localPosition.y;

        while (true)
        {
            Debug.Log(_posY);
            /* 
             * 곡선 이동을 위한 함수 정의
             * 시작 값부터 목적 값가지 특정한 비율로 증가, 감소함
             */
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            /* 프레임 횟수 증가 */
            cnt++;
            /* 고간 이동이 특정 프레임동안 지속된 이후 반복문 종료 */
            if (cnt > 15) break;
            /* 
             * 카메라 위치 이동 
             * x, z 축 위치는 변함이 없으므로 0으로 입력
             */
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            /* 한 프레임 대기(대기시간 없음) -> 매 프레임마다 위 과정을 실행 */
            yield return null;
        }

        /* 
         * 특정 프레임이 지나면 목적 위치로 이동하도록 정의
         * 기존 이동은 정확한 위치에 도달하지 못하기 때문에 그것을 보완하기 위함
         */
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0);
    }
}
