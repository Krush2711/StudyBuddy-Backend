#include<iostream>

using namespace std;

int main(){
    int a, b;
    cout<<"Enter the value of rows :"<<endl;
    cin >> a;
    cout<<"Enter the value of columns :"<<endl;
    cin >> b;
    for (int i=0; i<a ; i++){
        for(int j = 0; j<b; j++){
            cout<<"* ";
        }
        cout<<endl;
    }
}