using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static bool isActivate;

    [SerializeField]
    /* 현재 장착된 Hand 타입 무기 */
    private Hand currentHand;

    /* 공격 상태 파악 변수 */
    private bool isAttack;
    /* 팔 휘두름 상태 변수 */
    private bool isSwing;

    /*
     * Raycast 레이저를 통해 맞는 오브젝트의 정보를 가져울 수 있음
     * 그 때 사용하기 위한 변수
     */
    private RaycastHit hitInfo;

    // Update is called once per frame
    void Update()
    {
        if(isActivate)
        {
            /* 착용한 Hand를 통한 공격 시도 */
            TryAttack();
        }
    }

    private void TryAttack()
    {
        /* Unity 내에서 기본적으로 Ctrl1, Mouse 왼쪽 버튼은 Fire1로 지정되어 잇음 */
        if(Input.GetButton("Fire1"))
        {
            if(!isAttack)
            {
                //Coroutine 실행
                /* 마우스 클릭을 통한 중복 실행을 방지하기 위함 */
                StartCoroutine(AttackCoroutine());
            }
        }
    }
    private bool CheckObject()
    {
        /* 
         * 캐릭터 위치에서 전방으로 레이저를 발사함
         * transform.forward == transform.TransformDirection(Vector3.forward)
         * Vector3.forword = (0, 0, 1)
         *  -> 캐릭터는 위치가 계속해서 변함
         *  -> 그것을 TransformDirection 함수를 사용해 캐릭터 위치로 변경
         * 세 번째 매개변수는 충돌체가 있을 경우 hitInfo 변수에 충돌한 오브젝트의 정보를 받아옴
         * 네 번째 매개변수는 레이저의 길이를 설정
         */
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
            return true;
        return false;
    }

    /* 공격 딜레이를 적용하기 위해 Coroutine을 사용 */
    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        /* 애니메이션에 존재하는 Attack 트리거를 발동시킴 */
        currentHand.anim.SetTrigger("Attack");
        /* 공격 상태 적용 딜레이 적용 */
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        /* 공격 상태 제거 딜레이 적용 */
        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        /* 반복적인 공격을 방지하기 위한 딜레이 적용 */
        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    // 공격 Coroutine
    IEnumerator HitCoroutine()
    {
        while(isSwing)
        {
            /* 공격을 통해 충돌한 오브젝트가 있는지 확인 */
            if(CheckObject())
            {
                /* 공격에 적중한 오브젝트가 있을 경우 공격 상태를 종료 */
                isSwing = false;
                // 공격에 오브젝트가 충돌함
                Debug.Log("충돌 오브젝트 명 : " + hitInfo.transform.name);
            }
            /* 매 프레임 진행 */
            yield return null;
        }
    }

    // 현재 손 무기 변경 함수
    public void HandChange(Hand _hand)
    {
        /* 특정 물체를 들고 있는 경우 */
        if (WeaponManager.currentWeapon != null)
        {
            /* 이전에 활성화 되어있는 무기 오브젝트를 비활성화 함 */
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        /* 현재 무기를 변경 */
        currentHand = _hand;
        /* WeaponManager에 현재 무기 입력 */
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentHand.anim;
        /* 이전 무기에 의해 위치가 변경되었을 수도 있으므로 초기화 */
        currentHand.transform.localPosition = Vector3.zero;
        /* 변경된 무기 오브젝트를 활성화 함 */
        currentHand.gameObject.SetActive(true);
        /* 현재 손 무기를 사용하고 있음을 표시 */
        isActivate = true;
    }
}
