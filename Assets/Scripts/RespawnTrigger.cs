using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterController>();
        Respawn(character);
    }

    private void Respawn(CharacterController character)
    {
        character.enabled = false;
        character.transform.position = respawnPoint.position;
        character.enabled = true;
    }
}
