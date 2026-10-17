#include <iostream>

using namespace std;

int main()
{
    int a, b;
    cout << "Enter the value of rows and columns :" << endl;
    cin >> a;
    
    for (int i = 0; i < a; i++)
    {
        if (i == 0 || i == a - 1)
        {
            for (int j = 0; j < a; j++)
            {
                cout << "* ";
            }
            cout << endl;
        }
        
        else{
            cout<<"* ";
            for (int j = 0;j<a-2; j++){
                cout<<"  ";
            }
            cout<<"* "<<endl;
        }
    }
}