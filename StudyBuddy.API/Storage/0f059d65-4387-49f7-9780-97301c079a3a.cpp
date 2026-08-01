#include <iostream>
#include <vector>

using namespace std;

void print(vector<int> arr)
{
    for (int i = 0; i < arr.size(); i++)
    {
        cout << arr[i] << " ";
    }
    cout << endl;
}

void merger(vector<int> arr, vector<int> crr, vector<int>&uni)
{
    for (int i = 0; i < arr.size(); i++)
    {
        uni.push_back(arr[i]);
    }
    for (int i = 0; i < crr.size(); i++)
    {
        uni.push_back(crr[i]);
    }
}

int main()
{
    vector<int> arr{1, 3, 5, 7, 9};
    vector<int> crr{2, 4, 6, 8, 10};
    vector<int> uni;

    merger(arr, crr, uni);
    print(uni);
}