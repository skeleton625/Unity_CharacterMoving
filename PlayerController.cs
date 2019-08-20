using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* 
     * SerializeField - private은 유지되면서 인스펙터 창에 표시되는 변수 값 변경이 가능
     * * 단 모든 변수가 인스펙터 창에 표시되는 것은 아님
     */
    [SerializeField]
    /* 캐릭터의 이동 속도 */
    private float walkSpeed;

    /* 캐릭터의 물리적 몸체 - 충돌 영역 */
    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    {
        /* Script가 넣어진 오브젝트의 Rigidbody를 가져옴 */
        myRigid = GetComponent<Rigidbody>();
    }

    // 매 프레임( 초당 60 프레임 )마다 실행되는 함수
    void Update()
    {
        Move();
    }

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
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        /* 
         * Time.deltaTime -> 1 프레임의 대략적인 시간
         * 1초( 60 프레임 )동안 _velocity 만큼의 위치를 이동하게 됨
         */
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
}
