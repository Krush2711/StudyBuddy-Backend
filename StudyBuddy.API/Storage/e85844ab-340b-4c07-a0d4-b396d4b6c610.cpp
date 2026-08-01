#include <iostream>

using namespace std;

int getsum()
{
    cout << "Enter the value of n : ";
    int n;
    cin >> n;
    int sum = 0;

    for (int i = 0; i <= n; i++)
    {
        sum = i + sum;
    }

    return sum;
}

int main(){
int sum = getsum();
cout << sum ;
}