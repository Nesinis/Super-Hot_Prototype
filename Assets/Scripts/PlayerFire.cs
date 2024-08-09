using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab; // �߻��� �Ѿ��� ������
    public GameObject firePosition; // �Ѿ��� �߻�� ��ġ
    public GameObject PlayerPistol; // �÷��̾ ������ �ǽ��� ������Ʈ
    public GameObject pistolPrefab; // ���� �ǽ����� ������

    public Transform throwPos; // �ǽ����� ���� ��ġ
    public float throwPower = 20f; // �ǽ����� ������ ��
    public float punchRange = 0.2f; // ���� ���� ����

    void Update()
    {
        // Fire1 ��ư�� ������ ��
        if (Input.GetButtonDown("Fire1"))
        {
            if (PlayerPistol == null)
            {
                // �ǽ����� ������ ���� ������ ����
                FistAttack();
            }
            else
            {
                // �ǽ����� ������ ���� �߻�
                bulletFire1();
            }
        }

        // ���콺 ������ ��ư�� ���Ȱ� �ǽ����� ���� ��
        if (Input.GetMouseButtonDown(1) && PlayerPistol != null)
        {
            // �ǽ����� ����
            throwPistol();
        }
    }

    // ���� ������ �����ϴ� �޼ҵ�
    void FistAttack()
    {
        RaycastHit hit;
        // �÷��̾��� �������� ����ĳ��Ʈ�� ��� �浹 ���θ� Ȯ��
        if (Physics.Raycast(transform.position, transform.forward, out hit, punchRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // �浹�� ��ü�� �̸��� �α׷� ���

            // �浹�� ��ü�� 'Enemy' �±׸� ������ �ִ� ���
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                // ���� EnemyAI ������Ʈ�� ������
                EnemyAI enemyAI = hit.collider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // ���� ���� ���·� ����� ���� ������ ��
                    StartCoroutine(enemyAI.Stun());  // Enemy ���� ����
                    enemyAI.throwEnemyPistol();  // ���� ���� ������ �Լ� ȣ��
                    Debug.Log("Enemy stunned and pistol thrown: " + hit.collider.gameObject.name); // ���� �� �ǽ��� ������ ���� ����� �޽���
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any enemy."); // ���� ������ ���� ��� �α� ���
        }
    }

    // ���� �߻��ϴ� �޼ҵ�
    void bulletFire1()
    {
        // Fire1 ��ư�� ������ ��
        if (Input.GetButtonDown("Fire1"))
        {
            // ���� ī�޶��� ���� �������� �Ѿ��� �߻�
            Vector3 shootDirection = Camera.main.transform.forward;
            // �Ѿ� �ν��Ͻ��� �����ϰ� ��ġ�� ȸ���� ����
            GameObject bulletInstance = Instantiate(bulletPrefab, firePosition.transform.position, Quaternion.identity);
            BulletMove bulletMove = bulletInstance.GetComponent<BulletMove>();
            if (bulletMove != null)
            {
                // �Ѿ˿� �߻� ���� ����
                bulletMove.SetDirection(shootDirection);
            }
        }
    }

    // �ǽ����� ������ �޼ҵ�
    void throwPistol()
    {
        if (PlayerPistol != null)
        {
            // �÷��̾� �ǽ����� �ı��ϰ� ������ �ʱ�ȭ
            Destroy(PlayerPistol);
            PlayerPistol = null; // ���� �ʱ�ȭ

            // �ǽ����� ���� ��ġ���� �ν��Ͻ��� ����
            GameObject thrownPistol = Instantiate(pistolPrefab, firePosition.transform.position, Quaternion.identity);
            Rigidbody rb = thrownPistol.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���� ������ �����ϰ� ���� ���Ͽ� �ǽ����� ����
                Vector3 throwDir = (firePosition.transform.forward + firePosition.transform.up * 0.3f).normalized;
                rb.AddForce(throwDir * throwPower, ForceMode.Impulse);
            }
        }
    }
}
