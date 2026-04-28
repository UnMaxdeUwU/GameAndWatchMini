using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private Transform[] m_transforms;
   [SerializeField] private InputPlayerManagerCustomGameAndWatch m_inputManager;
   [SerializeField] private ObjectMovement[] m_objectMovement;
   [SerializeField] private SpriteRenderer m_spriteRenderer;
   [SerializeField] private Sprite m_spriteLeft;
   [SerializeField] private Sprite m_spriteRight;
   [SerializeField] private Sprite m_spriteIdle;

   public Action ObjectSmash;

   public int m_currentIndex = 2;
   private int m_moveSpeed = 1;

   private void OnEnable()
   {
      m_inputManager.OnMoveLeft += MoveToNextPosition;
      m_inputManager.OnMoveRight += MoveToPreviousPosition;
      foreach (var line in m_objectMovement)
         line.indexChange += CheckSmash;
   }

   private void OnDisable()
   {
      m_inputManager.OnMoveLeft -= MoveToNextPosition; // ⚠️ Bug fix : était += au lieu de -=
      m_inputManager.OnMoveRight -= MoveToPreviousPosition;
      foreach (var line in m_objectMovement)
         line.indexChange -= CheckSmash;
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
      SetSprite(m_spriteLeft);
      GameAndWatchAudioEvents.RaisePlayerMove();
   }

   public void MoveToPreviousPosition()
   {
      m_currentIndex -= m_moveSpeed;
      m_currentIndex = Mathf.Clamp(m_currentIndex, 0, m_transforms.Length - 1);
      UpdatePosition();
      SetSprite(m_spriteRight);
      GameAndWatchAudioEvents.RaisePlayerMove();
   }

   public void MoveToDirection(int direction)
   {
      m_currentIndex = m_currentIndex + m_moveSpeed * direction;
      m_currentIndex = Mathf.Clamp(m_currentIndex, 0, m_transforms.Length - 1);
      UpdatePosition();
      SetSprite(direction == -1 ? m_spriteLeft : m_spriteRight);
      GameAndWatchAudioEvents.RaisePlayerMove();
   }

   public void UpdatePosition()
   {
      transform.position = m_transforms[m_currentIndex].position;
   }

   private void SetSprite(Sprite sprite)
   {
      if (m_spriteRenderer != null)
         m_spriteRenderer.sprite = sprite;
   }

   private void CheckSmash(int idLine, int objectIndex)
   {
      Debug.Log("LineId" + idLine + "Object Index" + objectIndex);
      if (objectIndex == 4 && m_currentIndex == idLine)
      {
         Debug.Log("Object smash !");
         ObjectSmash?.Invoke();
      }
   }
}