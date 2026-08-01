#include <iostream>

using namespace std;

void count(int arr[], int size)
{
    int count_of_1 = 0;
    int count_of_0 = 0;

    for (int i = 0; i < size; i++)
    {
        if (arr[i]==1)
        {
            count_of_1++;
        }

        if (arr[i]==0)
        {
            count_of_0++;
        }
        
    }
    cout<<"Numbers of  1 is "<<count_of_1<<endl;
    cout<<"Numbers of  0 is "<<count_of_0<<endl;
}

int main()
{
    int arr[] = {1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 3, 5, 7, 9, 2, 4, 6};
    int size = sizeof(arr)/ sizeof(arr[0]);
    // cout<<size;
    //int key; cin>>key;

    int a = 5; 
    cout<<a++ + ++a;

    // count(arr, size);
}