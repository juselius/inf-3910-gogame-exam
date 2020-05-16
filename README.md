# Exam INF-3910-5, spring 2020

In this exercise you will implement a slightly simplified Go game. Go is an
ancient abstract strategy game for two players:

* https://en.wikipedia.org/wiki/Go_(game)
* https://en.wikipedia.org/wiki/Rules_of_Go

The game will be implemented using F# for both the client and the server, using
the provided starter code.

## Go Rules

The following rules apply for his exercise:

* **Rule 1** (the rule of liberty) states that every stone remaining on the board
  must have at least one open "point" (an intersection, called a "liberty")
  directly orthogonally adjacent (up, down, left, or right), or must be part of
  a connected group that has at least one such open point ("liberty") next to
  it. Stones or groups of stones which lose their last liberty are removed from
  the board.
* **Rule 2** (the "ko rule") states that the stones on the board must never
  repeat a previous position of stones. Moves which would do so are
  forbidden, and thus only moves elsewhere on the board are permitted that
  turn. (Hint: checksums)
* **Ending** the game happens when there are no more valid moves available, or
  when both players pass their turn consecutively.
* **Scoring** can be done in many ways. We will use the simplest method called
  stone scoring, where each player's score is the number of stones they hold at
  the end of the game.
* **Game board**: For simplicity we will use a 9x9 board for the exercise.

## Exercises

### Using the provided starter code

In steps 1-4 you can ignore above Go rules 1 and 2, only in step 5
you need to consider the Go rules.

1. Implement a game board in the client (browser). Full score for
   a proper Go style board, where stones are placed on the intersections.
   (Hint: tables and css) **6 p**
2. Implement placing of stones on the board such that all connected clients
   can participate and get updates (Hint: see the provided websocket handlers).
   **6 p**
3. Implement basic game play **10 p**:
    * Only two players can play (many can watch).
    * Consecutive turns, black starts.
    * Already occupied intersections are not playable.
4. Implement passing, game end and scoring. The game state (waiting for player,
   playing, turn, pass, win, tie) should be reported to connected clients.
   **4 p**
5. Implement the (simplified) Go rules as stated above. **10 p**

### Documentation

A *very short* written report must be included. The purpose is to help the
review process. It should list what has been done and what has not, what is
working and what is not. It should also mention anything else of importance,
like if the code is not compiling, design choices, etc.

You will not be graded for the report. But not delivering are report or
misreporting can cause a deduction of points. And remember, keep it short!

## Rules for the exam

The exercise is provided as a home exam, due **7.6.2020**.

1. Use the starter code available in GitHub classroom
2. Any modification to files marked *DO NOT MODIFY* ignored.
3. Questions must be asked in the open on Canvas.
4. The code should compile and run using `fake build -t run`.
5. Create a separate branch for each step, i.e. `step-N` where `N` is the step
   number.
6. The functionality of each step will be automatically tested using
   Canopy/Selenium. Each active element must be tagged with `prop.id` accodingly:
   * Play White button: `play-white`
   * Play Black button: `play-black`
   * Pass: `pass-turn`
   * Empty grid element: `empty-row-col` where `row` and `col` are integers
     between 0 and 8
   * Black grid element: `black-row-col`
   * White grid element: `white-row-col`
7. The exam must be submitted on WiseFlow as a pdf document with the code
   archive as an attachment:
   1. The submitted code should be cleaned from the following directories:
   `node_modules .fake .fable deploy src/*/bin src/*/obj`
   2. The archive should contain the `.git` folder.
   3. The archive should unpack into a folder with the candidates identifier,
   e.g. `mv inf-3910-gogame 123; tar fcz 123.tgz 123`
8. If you find a bug in the starter code, please let me know, and I'll post a fix.
9. Any updates to the starter code or to the exercise will be posted to the
   starter repository on GitHub and announced on Canvas.

## Grading

The grading of the exam will be based on the following criteria:

* Functionality, i.e. what works and what does not.
* Usage of functional constructs and functional style.
* Cleanliness of the code and project.

Each step will be separately tested for functionality. Since each step builds on
the previous step, only the final step (`step-5` or `master` branch) will be
graded by hand.

## Starter code

The provided starter code contains a skeleton client-server project. The
project should be compiled and run using `fake build -t run`. Do not modify
files marked as so.

In order to communicate (push) updates from the server to the client(s), the
starter code uses websockets. The current implementation only supports
sending messages from the server to the client. The client should use normal
REST calls to the server. The implementation supports broadcasting updates to
*all* connected clients, and sending update to a specific client using it's
connection id.

Functionality tests will be released in advance, so that you can test the
functionality for your self.

## Hints

You can get valuable help and inspiration from the following articles:

* https://fsharpforfunandprofit.com/cap/
* https://fsharpforfunandprofit.com/posts/enterprise-tic-tac-toe/
* https://fsharpforfunandprofit.com/posts/enterprise-tic-tac-toe-2/

When implementing the game play, using private or incognito mode in the
browser is a great way to test multiple users.

*Good luck, and may the Foo be with you!*
