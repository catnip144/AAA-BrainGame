using UnityEngine;

public class CrocSensor : MonoBehaviour
{
    [SerializeField] private Animator crocAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "CrocNumber")
        {
            Destroy(collision.gameObject, 0.2f);
            crocAnimator.Play("MunchNumber", -1, 0f);
        }
    }
}
