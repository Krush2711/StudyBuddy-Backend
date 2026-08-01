#include<iostream>
#include<climits>
using namespace std;

int main(){
   int n;
   cin>>n;

    int arr[n];
    for (int i = 0; i < n; i++)
    {
        cin>>arr[i];
    }
    
    int maxNo = INT_MIN;// this gives lowest value , so whenever  value grater than it shows up, it gets
    int minNo = INT_MAX;// lowest value.

    for (int i = 0; i < n; i++)
    {
       maxNo = max(maxNo, arr[i]);
       minNo = min(minNo, arr[i]);
    }
    
    cout<< maxNo <<" "<< minNo <<endl;

   

   return 0;
}