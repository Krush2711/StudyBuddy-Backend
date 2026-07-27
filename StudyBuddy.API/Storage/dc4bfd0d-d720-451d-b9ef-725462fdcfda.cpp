#include <iostream>

using namespace std;

int main()
{
    int row;
    cout << "Enter the numbers of rows : ";
    cin >> row;

    for (int i = 0; i < row; i++)
    {
        if (i == row - 1)
        {
            break;
        }

        else
        {
            for (int j = 0; j < i + 1; j++)
            {
                cout << "*";
            }
            for (int j = 0; j < 2 * (row - i - 1) - 1; j++)
            {
                cout << " ";
            }
            for (int j = 0; j < i + 1; j++)
            {
                cout << "*";
            }
            cout << endl;
        }
    }

    for (int i = 0; i < row; i++)
    {
        if (i == 0)
        {
            for (int j = 0; j < 2 * row - 1; j++)
            {
                cout << "*";
            }
            cout << endl;
        }

        else
        {
            for (int j = 0; j < row - i; j++)
            {
                cout << "*";
            }
            for (int j = 0; j < 2 * i - 1; j++)
            {
                cout << " ";
            }
            for (int j = 0; j < row - i; j++)
            {
                cout << "*";
            }
            cout << endl;
        }
    }
}