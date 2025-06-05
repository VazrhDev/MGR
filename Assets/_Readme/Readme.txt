
Missing Scripts issue for replay system:

1. Add ReplyPlayer script in main player prefab.
2. Add ReplayManager script in Replay Manager object in MainGameScene(Lobby Test).
3. Delete Replay Viewer missing prefab from ReplayScene(Replay Test).
4. Add ReplayViewer script in Replay Car prefab.
5. Provide references in Replay Manager accordingly.
6. Give appropriate onClick and onvalue changed references to button and sliders.