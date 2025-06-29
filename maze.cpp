#include <iostream>
#include <vector>
#include <cstdlib>
#include <ctime>
#include <algorithm>

const int N = 49;
int maze[N][N];

// 方向偏移：上、下、左、右
const int dx[4] = {-2, 2, 0, 0};
const int dy[4] = {0, 0, -2, 2};

bool isValid(int x, int y)
{
    return x > 0 && y > 0 && x < N - 1 && y < N - 1;
}

// 打乱方向
std::vector<int> shuffleDirections()
{
    std::vector<int> dirs = {0, 1, 2, 3};
    std::random_shuffle(dirs.begin(), dirs.end());
    return dirs;
}

void dfs(int x, int y)
{
    maze[x][y] = 0;

    auto dirs = shuffleDirections();
    for (int dir : dirs)
    {
        int nx = x + dx[dir];
        int ny = y + dy[dir];
        int mx = x + dx[dir] / 2;
        int my = y + dy[dir] / 2;

        if (isValid(nx, ny) && maze[nx][ny] == 1)
        {
            maze[mx][my] = 0; // 打通中间墙
            dfs(nx, ny);
        }
    }
}

// 输出为 Unity 可用格式
void printMazeUnityFormat()
{
    std::cout << "{\n";
    for (int i = 0; i < N; i++)
    {
        std::cout << "  {";
        for (int j = 0; j < N; j++)
        {
            std::cout << maze[i][j];
            if (j < N - 1)
                std::cout << ", ";
        }
        std::cout << "}";
        if (i < N - 1)
            std::cout << ",";
        std::cout << "\n";
    }
    std::cout << "}\n";
}

int main()
{
    srand(time(0));

    // 初始化为墙
    for (int i = 0; i < N; ++i)
        for (int j = 0; j < N; ++j)
            maze[i][j] = 1;

    // 中心 3x3 出生点清空
    for (int i = 23; i <= 25; i++)
        for (int j = 23; j <= 25; j++)
            maze[i][j] = 0;

    dfs(24, 24); // 从中心开始打通

    // 设置边界为墙（再次覆盖）
    for (int i = 0; i < N; ++i)
        maze[i][0] = 1, maze[i][N - 1] = 1;
    for (int j = 0; j < N; ++j)
        maze[0][j] = 1, maze[N - 1][j] = 1;

    // 在左边界随机找一个连通的地方作为出口
    std::vector<int> candidates;
    for (int i = 1; i < N - 1; i++)
        if (maze[i][1] == 0)
            candidates.push_back(i);

    if (!candidates.empty())
    {
        int exitY = candidates[rand() % candidates.size()];
        maze[exitY][0] = 0;
    }

    printMazeUnityFormat();
    return 0;
}
