#include <iostream>
#include <ostream>
#include <typeinfo>
#include <vector>
#include <memory>
#include <ctime>
#include <cstdlib>

#include "Character.h"
#include "Seeker.h"
#include "Templar.h"

using std::cout;
using std::endl;
using std::unique_ptr;
using std::make_unique;
using std::vector;

int main() {
  std::srand(static_cast<unsigned>(std::time(nullptr)));
  vector<unique_ptr<Character>> party;
  party.push_back(make_unique<Seeker>("Искатель1", 10, 100));
  party.push_back(make_unique<Seeker>("Искатель2", 10));
  party.push_back(make_unique<Seeker>("Искатель3"));
  party.push_back(make_unique<Templar>("Храмовник1", 7, 100));
  party.push_back(make_unique<Templar>("Храмовник2", 10));
  party.push_back(make_unique<Templar>("Храмовник3"));
  unique_ptr<Character> _seeker = make_unique<Templar>("Искатель2", 10);
  cout << "\n" << endl;
  // party[1]->useAbility();
  // party[1]->useSpecial();
  // party[1]->attack(*party[3]);
  // party[1]->takeDamage(50);
  // party[1]->printInfo();
  // cout << "" << endl;
  // static_cast<Seeker *>(party[1].get())->enterStealth();
  // cout << "" << endl;
  // static_cast<Seeker *>(party[1].get())->showSeekerStats();
  // cout << "" << endl;
  cout << "static:" << endl;
  static_cast<Seeker *>(party[4].get())->enterStealth();
  if (static_cast<Seeker *>(party[4].get())->tryDodge()) {
    cout << static_cast<Seeker *>(party[4].get()) << endl;
  } else {
    cout << "static выдал ошибку" << endl;
  }
  cout << "dynamic:" << endl;
  dynamic_cast<Seeker *>(party[4].get())->enterStealth();
  if (dynamic_cast<Seeker *>(party[4].get()) == nullptr) {
    cout << "dynamic вернул nullptr" << endl;
  } else {
    cout << dynamic_cast<Seeker *>(party[1].get()) << endl;
  }
      // g++ -std=c++17 -Wall -Wextra -O2 main.cpp Character.cpp
      // MeleeCharacter.cpp Seeker.cpp Templar.cpp -o app.exe

      return 0;
}
