#!/bin/bash

# Move into the Databases folder
if ! [[ -d ./Databases ]]
then
    echo "Logging folder doesn't exist, creating ../Databases"
    mkdir Databases
else
    echo "./Databases exists"
fi
cd ./Databases

# Move into the Dbs folder
if ! [[ -d ./Logging ]]
then
    echo "Logging folder doesn't exist, creating ./Logging"
    mkdir Logging
else
    echo "../Databases/Logging exists"
fi
cd ./Logging

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
echo "$APIKeysTable" | $db
echo "Finished creating the Apps.db"