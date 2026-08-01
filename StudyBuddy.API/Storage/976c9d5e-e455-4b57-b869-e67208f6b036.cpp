#include <iostream>

using namespace std;

void linearSearch(int arr[], int size, int key)
{

    for (int i = 0; i < size; i++)
    {
        if (i == size - 1)
        {
            if (arr[i] == key)
            {
                cout << "Element found " << arr[i] << endl;
                break;
            }
            else
            {
                cout << "Elemnet is not found !";
            }
        }
        if (arr[i] == key)
        {
            cout << "Element found " << arr[i] << endl;
            break;
        }
    }
}

int main()
{
    int arr[] = {1, 2, 3, 4, 5, 6};
    int size = sizeof(arr)/ sizeof(arr[0]);
    cout<<size;
    int key; cin>>key;

    linearSearch(arr, size, key);
}