using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private Transform[] m_transforms;
   [SerializeField] private InputPlayerManagerCustomGameAndWatch m_inputManager; // lien component/event dispatcher
   [SerializeField] private ObjectMovement[] m_objectMovement;
   public Action ObjectSmash;
   
   
   public int m_currentIndex = 2;
   private int m_moveSpeed = 1;
   

   private void OnEnable()
   {
      m_inputManager.OnMoveLeft += MoveToNextPosition; // bind à event dispatcher left
      m_inputManager.OnMoveRight += MoveToPreviousPosition;
      foreach (var line in m_objectMovement)
      {
         line.indexChange += CheckSmash;
      }
         
   }

   private void OnDisable()
   {
      m_inputManager.OnMoveLeft += MoveToNextPosition; // Unbind à event dispatcher left
      m_inputManager.OnMoveRight += MoveToPreviousPosition;
      foreach (var line in m_objectMovement)
      {
         line.indexChange -= CheckSmash;
      }
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

   private void CheckSmash(int idLine, int objectIndex)
   {
      Debug.Log("LineId" + idLine +"Object Index"  + objectIndex);
      if (objectIndex == 4 && m_currentIndex ==  idLine)
      {
         Debug.Log("Object smash !");
         ObjectSmash?.Invoke();
         
      }
   }
   
}
