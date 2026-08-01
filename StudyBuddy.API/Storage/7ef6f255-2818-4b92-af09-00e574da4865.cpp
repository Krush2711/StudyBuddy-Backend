#include <iostream>

using namespace std;

void print(int arr[], int s)
{
    for (int i = 0; i < s; i++)
    {
        cout << arr[i] << " ";
    }
}

void max_of(int arr[], int size)
{

    int a = INT16_MIN;

    for (int i = 0; i < size; i++)
    {
        if (arr[i] > a)
        {
            a = arr[i];
            
        }
    }cout<<"MAx number is :"<< a <<endl;

}

int main()
{
    int arr[] = {1, 89, 34, 76, 21, 5, 90, 12, 67, 3, 45, 56, 77, 88, 92, 41, 30, 6, 72, 18, 53, 27, 11, 65};
    int size = sizeof(arr) / sizeof(arr[0]);
    // cout << ?;

    max_of(arr, size);
}