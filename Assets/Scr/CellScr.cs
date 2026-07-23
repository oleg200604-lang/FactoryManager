using UnityEngine;

public class CellScr : MonoBehaviour
{
    public IBuilding buld;

    public bool IsEmpty => buld == null;

    public void SelectBuld(IBuilding newBuld)
    {
        if (newBuld == null) return;

        if (!IsEmpty)
        {
            Debug.Log($"Клітинка {name} вже зайнята будівлею {buld.GetType().Name}.");
            return;
        }

        buld = newBuld;
        Debug.Log($"Будівлю {newBuld.GetType().Name} розміщено на клітинці {name}.");
    }

    public void RemoveBuld()
    {
        buld = null;
    }
}