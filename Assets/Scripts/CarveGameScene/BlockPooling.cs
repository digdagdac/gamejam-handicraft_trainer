using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BlockPooling : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas _atlas;

    private Queue<Block> _activeBlock = new Queue<Block>(20);
    private Queue<Block> _offBlock = new Queue<Block>(20);

    private readonly Vector2 _initPos = new Vector2(0, 1080);
    private int _totalCount;

    public void Init(int blockCount)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            _offBlock.Enqueue(transform.GetChild(i).GetComponent<Block>());
        }

        Block block = _offBlock.Dequeue();
        block.Init(_atlas, _initPos, blockCount, 0);
        _activeBlock.Enqueue(block);
        _totalCount = blockCount;
    }

    public void SetBlock(int num)
    {
        _activeBlock.Peek().SetEffect(num);

        if (num % 5 == 0 && num < _totalCount)
        {
            _offBlock.Enqueue(_activeBlock.Dequeue());
            Block block = _offBlock.Dequeue();
            block.Init(_atlas, _initPos, _totalCount, num);
            _activeBlock.Enqueue(block);
        }
    }
}
