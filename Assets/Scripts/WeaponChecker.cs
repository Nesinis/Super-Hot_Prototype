using UnityEngine;

public class WeaponChecker : MonoBehaviour
{
    public GameObject pistol2; // 플레이어나 적이 들고 있는 총

    public bool CheckGunPresence()
    {
        if (pistol2 != null)
        {
            return pistol2.activeInHierarchy; // 총의 존재 여부를 반환
        }
        else
        {
            return false; // pistol2가 null이면 총이 없는 상태로 설정
        }
    }
}
