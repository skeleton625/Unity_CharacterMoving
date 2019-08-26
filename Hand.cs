using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    /* 특정 상태의 핸드 명 */
    public string handName;
    /* 공격 범위 */
    public float range;
    /* 공격 피해량 */
    public int damage;
    /* 손으로 일하는 속도 */
    public float workSpeed;
    /* 공격 한 번당 지연되는 시간 */
    public float attackDelay;
    /* 공격 활성화 시점 */
    public float attackDelayA;
    /* 공격 비활성화 시점 */
    public float attackDelayB;
    /* 공격 기준을 핸드가 아닌 화면으로 지정하기 위한 애니메이션 */
    public Animator anim;

    /*
     * Start, Update 함수는 있는 것 만으로도 자원을 소모함
     */
}
