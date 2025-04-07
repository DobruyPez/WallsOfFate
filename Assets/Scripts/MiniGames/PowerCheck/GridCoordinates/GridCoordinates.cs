using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    public class GridCordinates : MonoBehaviour
    {
        public int MatrixWidth = 20; // ���������� ������� � �������
        public int MatrixHeight = 20; // ���������� ����� � �������
        public float CellWidth = 1; // ������ ����� ������
        public float CellHeight = 1; // ������ ����� ������
        public Transform GridCenter; // ��������� ������ �������
        public Transform ParentForAllCells; // ������������ ������ ��� ���� ������

        public List<List<GridCordEl>> CordMatrix { get; private set; } // ������� ���������

        private void Awake()
        {
            InitializeMatrix();
            DrawMatrix();

            GridCordEl gridCordEl = FindCellsByPosition(new Vector3(9.4f, 0, 9.4f));
            Debug.Log(gridCordEl.Center);
        }

        /// <summary>
        /// ������������� ������� ���������
        /// </summary>
        public void InitializeMatrix()
        {
            CordMatrix = new List<List<GridCordEl>>();

            for (int i = 0; i < MatrixHeight; i++)
            {
                var row = new List<GridCordEl>();
                for (int j = 0; j < MatrixWidth; j++)
                {
                    Vector2 pos = Vector2.zero;
                    GridCordEl gridel = new GridCordEl(CellWidth, CellHeight, TypeOfCordEl.NotOccupied, i, j);
                    row.Add(gridel); 
                }
                CordMatrix.Add(row);
            }
        }

        /// <summary>
        /// ��������� ������� � ������� ����
        /// </summary>
        public void DrawMatrix()
        {
            if (GridCenter == null)
            {
                Debug.LogError("����� ������� �� �����!");
                return;
            }

            Vector3 startPosition = GridCenter.position - new Vector3(
                (MatrixWidth * CellWidth) / 2f,
                0,
                (MatrixHeight * CellHeight) / 2f); // ������� ����� ���� ������� ������������ ������

            for (int i = 0; i < MatrixHeight; i++)
            {
                for (int j = 0; j < MatrixWidth; j++)
                {
                    // ��������� ������� ������ ������
                    Vector3 cellPosition = startPosition + new Vector3(
                        j * CellWidth + CellWidth / 2f,
                        -0.1f,
                        i * CellHeight + CellHeight / 2f);
                    
                    DrawCell(cellPosition, i, j, ParentForAllCells);
                    CordMatrix[i][j].SetGlobalCenter(cellPosition);

                    Vector3 localPosition = ParentForAllCells.InverseTransformPoint(cellPosition);
                    CordMatrix[i][j].SetCenter(localPosition);
                }
            }
        }

        private GridCordEl FindCellsByPosition(Vector3 position)
        {
            Vector2 position2d = new Vector2(position.x, position.z);

            int maxRow = Mathf.RoundToInt(Mathf.Log(CordMatrix.Count) / Mathf.Log(2));

            int rowIndex = CordMatrix.Count / 2, ifincell = -1;
            int colIndex = 0;
            BinaryCycle(position2d, CordMatrix.Count, ref rowIndex, ref colIndex, ref ifincell, false);

            if (ifincell == 1) return CordMatrix[rowIndex][colIndex];
            else if (ifincell == 0)
            {
                colIndex = CordMatrix[rowIndex].Count / 2;
                int maxCol = Mathf.RoundToInt(Mathf.Log(CordMatrix[rowIndex].Count) / Mathf.Log(2));
                BinaryCycle(position2d, CordMatrix[rowIndex].Count, ref colIndex, ref rowIndex, ref ifincell, true);
                if (ifincell == 1) return CordMatrix[rowIndex][colIndex];
            }

            return null;
        }

        private void BinaryCycle(Vector2 position2d, int limit, ref int index, ref int secondaryIndex,
            ref int ifInCell, bool searchByX)
        {
            if (ifInCell == 0 || ifInCell == 1) ifInCell = -1; 
            int prevIndex = 0, numOfSteps = 0;
            while (index >= 0 && index < limit /*numOfSteps <= limit + 1*/)
            {
                if (ifInCell == 0 || ifInCell == 1) break;

                int step = Mathf.RoundToInt(Mathf.Abs(index - prevIndex) / 2);
                if(step == 0) step = 1;
                prevIndex = index;
                if (searchByX)
                {
                    float comparisonValue = CordMatrix[secondaryIndex][index].Center.x;
                    index += comparisonValue < position2d.x ? step : -step;
                    ifInCell = IfPointInCell(position2d, secondaryIndex, index, searchByX);
                }
                else
                {
                    float comparisonValue = CordMatrix[index][secondaryIndex].Center.y;
                    index += comparisonValue < position2d.y ? step : -step;
                    ifInCell = IfPointInCell(position2d, index, secondaryIndex, searchByX);
                }
                numOfSteps++;
            }
        }

        // ���������� 3 ���������
        // ���� ��� ���������� ������ � ������� ������ �� 1
        // ���� ������ ���� ���������� ������� � ������� �� 0
        // ����� -1
        private int IfPointInCell(Vector2 pointPosition, int rowIndex, int colIndex,  bool findRow)
        {
            if (IsOutOfBounds(rowIndex, colIndex)) return -1;

            float leftBorder = CordMatrix[rowIndex][colIndex].Center.x - CordMatrix[rowIndex][colIndex].Width / 2;
            float rightBorder = CordMatrix[rowIndex][colIndex].Center.x + CordMatrix[rowIndex][colIndex].Width / 2;
            float topBorder = CordMatrix[rowIndex][colIndex].Center.y - CordMatrix[rowIndex][colIndex].Height / 2;
            float bottomBorder = CordMatrix[rowIndex][colIndex].Center.y + CordMatrix[rowIndex][colIndex].Height / 2;

            bool xInRange = leftBorder <= pointPosition.x && pointPosition.x <= rightBorder;
            bool yInRange =  topBorder <= pointPosition.y && pointPosition.y <= bottomBorder;

            if (xInRange && yInRange && findRow) return 1;
            if ((xInRange || yInRange) && !findRow) return 0;

            return -1;
        }

        //public HashSet<GridCordEl> GetAdjacentCells(Vector2 pointPosition, float width, float height, int row, int col)
        //{
        //    HashSet<GridCordEl> adjacentCells = new HashSet<GridCordEl>();
        //    RecursivelyFindCells(pointPosition, width, height, row, col, adjacentCells);
        //    return adjacentCells;
        //}

        //private void RecursivelyFindCells(Vector2 pointPosition, float width, float height, int row, int col, HashSet<GridCordEl> adjacentCells)
        //{
        //    // �������� ������ �� �������
        //    if (row < 0 || row >= CordMatrix.Count || col < 0 || col >= CordMatrix[row].Count)
        //        return;

        //    GridCordEl currentCell = CordMatrix[row][col];

        //    // �������� �� �����������
        //    if (IfPointInCell(pointPosition, width, height, true))
        //    {
        //        adjacentCells.Add(currentCell);
        //    }

        //    // ����������� ����� ��� �������� ������
        //    RecursivelyFindCells(pointPosition, width, height, row - 1, col, adjacentCells); // ������� ������
        //    RecursivelyFindCells(pointPosition, width, height, row + 1, col, adjacentCells); // ������ ������
        //    RecursivelyFindCells(pointPosition, width, height, row, col - 1, adjacentCells); // ����� ������
        //    RecursivelyFindCells(pointPosition, width, height, row, col + 1, adjacentCells); // ������ ������
        //}

        private bool IsOutOfBounds(int row, int col)
        {
            return row < 0 || row >= CordMatrix.Count || col < 0 || col >= CordMatrix[row].Count;
        }

        private void DrawCell(Vector3 cellPosition, int i, int j, Transform parent)
        {
            // ��������� ������ (��������, ������� ���������� ������)
            GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cell.transform.position = cellPosition;
            cell.transform.localScale = new Vector3(CellWidth, 0.2f, CellHeight);
            cell.name = $"Cell[{i},{j}]";

            // ��������� ������������� ������� ��� ������
            if (parent != null)
                cell.transform.SetParent(parent);

            // ��������� ����� � ����������� �� ��������� (�� ��������� ��� �����)
            var renderer = cell.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.white;

            // �������� ������ ������
            float borderThickness = 0.1f; // ������� ������

            // ������� �������
            GameObject topBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topBorder.transform.position = cellPosition + new Vector3(0, 0.1f, CellHeight / 2f - borderThickness / 2f);
            topBorder.transform.localScale = new Vector3(CellWidth, borderThickness, borderThickness);
            topBorder.name = $"TopBorder[{i},{j}]";

            // ������ �������
            GameObject bottomBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomBorder.transform.position = cellPosition + new Vector3(0, 0.1f, -(CellHeight / 2f - borderThickness / 2f));
            bottomBorder.transform.localScale = new Vector3(CellWidth, borderThickness, borderThickness);
            bottomBorder.name = $"BottomBorder[{i},{j}]";

            // ����� �������
            GameObject leftBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftBorder.transform.position = cellPosition + new Vector3(-(CellWidth / 2f - borderThickness / 2f), 0.1f, 0);
            leftBorder.transform.localScale = new Vector3(borderThickness, borderThickness, CellHeight);
            leftBorder.name = $"LeftBorder[{i},{j}]";

            // ������ �������
            GameObject rightBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightBorder.transform.position = cellPosition + new Vector3(CellWidth / 2f - borderThickness / 2f, 0.1f, 0);
            rightBorder.transform.localScale = new Vector3(borderThickness, borderThickness, CellHeight);
            rightBorder.name = $"RightBorder[{i},{j}]";

            // ��������� ������������� ������� ��� ������
            topBorder.transform.SetParent(cell.transform);
            bottomBorder.transform.SetParent(cell.transform);
            leftBorder.transform.SetParent(cell.transform);
            rightBorder.transform.SetParent(cell.transform);

            // ��������� ����� ��� ������ (��������, ������)
            Color borderColor = Color.black;

            var topBorderRenderer = topBorder.GetComponent<Renderer>();
            if (topBorderRenderer != null)
                topBorderRenderer.material.color = borderColor;

            var bottomBorderRenderer = bottomBorder.GetComponent<Renderer>();
            if (bottomBorderRenderer != null)
                bottomBorderRenderer.material.color = borderColor;

            var leftBorderRenderer = leftBorder.GetComponent<Renderer>();
            if (leftBorderRenderer != null)
                leftBorderRenderer.material.color = borderColor;

            var rightBorderRenderer = rightBorder.GetComponent<Renderer>();
            if (rightBorderRenderer != null)
                rightBorderRenderer.material.color = borderColor;
        }

    }
}
