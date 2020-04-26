using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GravityBlocks
{
    // int[,] 를 받아서 직접 Scene 에 그려준다. 
    // 이 때, int[,]의 요소가 0 이면 비어있고, 0 보다 크면 특정 색의 블록이 존재한다는 의미이다. 
    // Screen size 를 고려해서, 한 블록( 정사각형 )의 크기를 고려한다. 
    public class FieldRenderer
    {
        int fieldWidth, fieldHeight;

        // 블록을 그릴 기준점이 될, [0,0] 점의 World Position
        Vector3 defaultWorldPosition;

        // 블록 한 변의 World 기준 길이 
        float blockEdgeSize = 0;

        // 렌더러들의 집합 
        SpriteRenderer[,] renderSet;

        // 인스턴스 생성시에 , Field 의 사이즈를 정한다. 
        public FieldRenderer(int blockColumnCount, int blockRowCount)
        {
            // 블록의 최대 변의 길이를 구해주고 , 꽉차지 않게 95% 만큼 줄여주었다. 
            blockEdgeSize = CalculateBlockEdgeSize(blockColumnCount, blockRowCount) * 0.95f;
            defaultWorldPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);

            renderSet = CreateRenderSet(blockColumnCount, blockRowCount, defaultWorldPosition, blockEdgeSize);
        }

        public void Draw(int[,] fieldData)
        {
            var inputDataWidth = fieldData.GetLength(0);
            var inputDataHeight = fieldData.GetLength(1);

            if (inputDataWidth != fieldWidth || inputDataHeight != fieldHeight)
            {
                throw new System.Exception("InputFiledDataSizeError");
            }
            for (int i = 0; i < inputDataWidth; i++)
            {
                for (int j = 0; j < inputDataHeight; j++)
                {
                    var index = fieldData[i, j];
                    renderSet[i, j].color = ConvertIndexToColor(index);
                }
            }
        }

        SpriteRenderer[,] CreateRenderSet(int column, int row, Vector3 defaultPosition, float blockEdgeSize)
        {
            var rendererSet = new SpriteRenderer[column, row];
            var emptyGameObject = new GameObject();
            var defaultSprite = CreateWhiteSprite();
            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    var targetPosition = defaultPosition;
                    // 중앙을 기준으로 피벗이 잡혀서, 반칸 만큼 더 밀어줬다. 
                    targetPosition.x += (float)i * blockEdgeSize + blockEdgeSize * 0.5f;
                    targetPosition.y += (float)j * blockEdgeSize + blockEdgeSize * 0.5f;
                    targetPosition.z = 0;

                    GameObject newRendeObject = GameObject.Instantiate(emptyGameObject);
                    newRendeObject.transform.position = targetPosition;
                    var spriteRenderer = newRendeObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = defaultSprite;

                    var renderSize = spriteRenderer.size.x;
                    var rate = blockEdgeSize / renderSize;
                    newRendeObject.transform.localScale *= rate;

                    rendererSet[i, j] = spriteRenderer;
                }
            }

            GameObject.Destroy(emptyGameObject);
            return rendererSet;
        }

        float CalculateBlockEdgeSize(int blockColumnCount, int blockRowCount)
        {
            // 현재 주 스크린의 크기를 구한다. 
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var screenPositionMax = new Vector2(screenWidth, screenHeight);

            // 메인 카메라를 기준으로, 스크린 내에 잡히는 최소 Position 과 최대 Position 을 구한다. 
            var mainCamera = Camera.main;
            var worldPositionMaxInScreen = mainCamera.ScreenToWorldPoint(screenPositionMax);
            var worldPositionMinInScreen = mainCamera.ScreenToWorldPoint(Vector3.zero);

            // 스크린의 너비와 높이를 World 기준으로 구한다. 
            var worldWidth = worldPositionMaxInScreen.x - worldPositionMinInScreen.x;
            var worldHeight = worldPositionMaxInScreen.y - worldPositionMinInScreen.y;

            // 가능한 최대의 한 블록의 너비, 높이를 구한다.
            // 블록은 정사각형이므로 , 더 작은 쪽을 기준으로 삼아야 한다.
            var maxBlockSizeX = worldWidth / (float)blockColumnCount;
            var maxBlockSizeY = worldHeight / (float)blockRowCount;
            if (maxBlockSizeX >= maxBlockSizeY)
            {
                return maxBlockSizeY;
            }
            else // if(maxBlockSizeX < maxBlockSizeY)
            {
                return maxBlockSizeX;
            }
        }

        Sprite CreateWhiteSprite()
        {
            var whiteTexture = Texture2D.whiteTexture;
            var rect = new Rect(0, 0, 4, 4);
            var pivot = new Vector2(0.5f, 0.5f);
            var whiteSprite = Sprite.Create(whiteTexture, rect, pivot);
            return whiteSprite;
        }

        Color ConvertIndexToColor(int index)
        {
            Color resultColor = Color.white;
            switch (index)
            {
                case 0:
                    resultColor = Color.white;
                    break;
                case 1:
                    resultColor = Color.red;
                    break;
                case 2:
                    resultColor = Color.blue;
                    break;
                case 3:
                    resultColor = Color.green;
                    break;
                case 4:
                    resultColor = Color.yellow;
                    break;
                default:
                    resultColor = Color.black;
                    break;
            }
            return resultColor;
        }
    }

}