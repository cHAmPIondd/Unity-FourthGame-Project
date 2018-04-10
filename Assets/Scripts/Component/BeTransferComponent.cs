using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeTransferComponent : MonoBehaviour {
    [SerializeField,Tooltip("传送后偏移多远")]
    private float m_Offset=0.5f;
    private Rigidbody2D m_Rigidbody2D;
    private PhysicsUpdate m_PhysicsUpdate;
    private float m_LastBeTransferTime;
    private Transform m_LastTransferGate;
    void Awake()
    {
        m_Rigidbody2D=GetComponent<Rigidbody2D>();
        m_PhysicsUpdate = GetComponent<PhysicsUpdate>();
    }
    public void BeTransfer(TransferController.Direction entryDirection, bool isSameDirection, Vector2 pos, Transform transferGate)
    {
        m_LastTransferGate = transferGate;
        m_LastBeTransferTime = Time.time;
        switch (entryDirection)
        {
            case TransferController.Direction.Left:

                transform.position = pos + new Vector2(isSameDirection ? -m_Offset : m_Offset, 0);
                if (isSameDirection)
                    m_PhysicsUpdate.InvertHorizontalVelocity();
                m_PhysicsUpdate.ResetVerticalVelocity();
                break;
            case TransferController.Direction.Right:
                transform.position = pos + new Vector2(isSameDirection ? m_Offset : -m_Offset, 0);
                if (isSameDirection)
                    m_PhysicsUpdate.InvertHorizontalVelocity();
                m_PhysicsUpdate.ResetVerticalVelocity();
                break;
            case TransferController.Direction.Up:
                transform.position = pos + new Vector2(0, isSameDirection ? m_Offset : -m_Offset);
                if (isSameDirection)
                    m_PhysicsUpdate.InvertVerticalVelocity();
                m_PhysicsUpdate.ResetHorizontalVelocity();
                break;
            case TransferController.Direction.Down:
                transform.position = pos + new Vector2(0, isSameDirection ? -m_Offset : m_Offset);
                if (isSameDirection)
                    m_PhysicsUpdate.InvertVerticalVelocity();
                m_PhysicsUpdate.ResetHorizontalVelocity();
                break;
        }
    }
    public bool CanBeTransfer(TransferController.Direction entryDirection,Transform transferGate)
    {
        if (m_PhysicsUpdate.IsDied)
            return false;
        if (!(m_LastTransferGate != transferGate || m_LastBeTransferTime + 0.3f < Time.time))
            return false;
        switch (entryDirection)
        {
            case TransferController.Direction.Left:
                if (m_Rigidbody2D.velocity.x > 0.1f)
                {
                    return true;
                }
                break;
            case TransferController.Direction.Right:
                if (m_Rigidbody2D.velocity.x < -0.1f)
                {
                    return true;
                }
                break;
            case TransferController.Direction.Up:
                if (m_Rigidbody2D.velocity.y <= 0f)
                {
                    return true;
                }
                break;
            case TransferController.Direction.Down:
                if (m_Rigidbody2D.velocity.y > 0.1f)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}