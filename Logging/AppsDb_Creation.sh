#!/bin/bash

# Move into the Dbs folder
if ! [[ -d ./Dbs ]]
then
    echo "Dbs doesn't exist, creating ./Dbs"
    mkdir Dbs
else
    echo "./Dbs exists"
fi
cd ./Dbs

# Apps.db database sqlite3 connection.
touch ./Apps.db
db="sqlite3 ./Apps.db"

# Create Apps
AppTable="
CREATE TABLE IF NOT EXISTS Apps(
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Name           TEXT      NOT NULL,
   IsDeleted BOOLEAN NOT NULL CHECK (IsDeleted IN (0, 1)) DEFAULT 0
);
"

APIKeysTable="
CREATE TABLE IF NOT EXISTS ApiKeys(
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Name           TEXT      NOT NULL,
   IsDeleted BOOLEAN NOT NULL CHECK (IsDeleted IN (0, 1)) DEFAULT 0
);
"

echo "$AppTable" | $db
echo "$APIKeysTable" | $dbx
echo "Finished creating the Apps.db"