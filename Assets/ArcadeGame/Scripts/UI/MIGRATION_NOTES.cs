// ComboController.cs est maintenant remplacé par le système modulaire :
//
//   ComboManager          → cerveau (calcul combo + events)
//   ComboUIFeedback       → compteur + rank splash UI
//   ComboVFXController    → aura + trails par tier
//   ComboCameraFeedback   → hitstop + shake scalés
//   ComboBurstSpawner     → burst visuel aux paliers
//
// Tu peux supprimer ComboController.cs de ton projet.
// SwordPlayer.ResetCooldownAndSpeed() est appelé par ComboManager.ResetCombo().
//
// Migration :
//   1. Ajoute ComboManager sur ton GameManager GO (assigne SwordPlayer + ComboConfig)
//   2. Ajoute ComboUIFeedback sur ton Canvas (assigne les deux TMP_Text + ComboConfig)
//   3. Ajoute ComboVFXController sur le joueur (assigne les prefabs VFX par tier)
//   4. Ajoute ComboCameraFeedback sur ton GameManager GO (assigne ComboConfig)
//   5. Ajoute ComboBurstSpawner sur le joueur (assigne prefabs burst + spawnPoint)
//   6. Retire les appels HitStop/CameraShake de SwordPlayer.ApplyHitFeedback
//      → ils sont maintenant gérés par ComboCameraFeedback
