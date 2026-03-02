using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private Transform[] m_transforms;
   [SerializeField] private InputPlayerManagerCustom m_inputManager; // lien component/event dispatcher

   private int m_currentIndex = 2;
   private int m_moveSpeed = 1;

   private void OnEnable()
   {
      m_inputManager.OnMoveLeft += MoveToPreviousPosition; // bind à event dispatcher left
      m_inputManager.OnMoveRight += MoveToNextPosition; // bind à event dispatcher right
   }

   private void Start()
   {
      m_currentIndex = 2;
      transform.position = m_transforms[m_currentIndex].position;
   }

   public void MoveToNextPosition()
   {
      m_currentIndex += m_moveSpeed;
      m_currentIndex = Mathf.Clamp(m_currentIndex, 0, m_transforms.Length - 1);
      UpdatePosition();
   }

   public void MoveToPreviousPosition()
   {
      m_currentIndex -= m_moveSpeed;
      m_currentIndex = Mathf.Clamp(m_currentIndex, 0, m_transforms.Length - 1);
      UpdatePosition();
   }

   public void MoveToDirection(int direction) // direction = -1 OU 1
   {
      m_currentIndex = m_currentIndex + m_moveSpeed*direction;
      m_currentIndex = Mathf.Clamp(m_currentIndex, 0, m_transforms.Length - 1);
      UpdatePosition();
   }

   public void UpdatePosition()
   {
      transform.position = m_transforms[m_currentIndex].position;
   }
}
