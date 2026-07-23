using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CellScr : MonoBehaviour
{
    public IBuilding buld;
    public Vector2Int gridPosition;

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
    [CustomEditor(typeof(CellScr))]
    public class CellScrEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CellScr cell = (CellScr)target;
            string buildingName = cell.buld != null ? cell.buld.GetType().Name : "Порожньо";
            EditorGUILayout.LabelField("Поточна будівля (runtime)", buildingName);
        }
    }
}