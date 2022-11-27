#!/bin/bash

if [[ $# != 1 ]]
then
    if [[ $# == 0 ]]
    then
        echo "No arguments passed"
    else
        echo "Too many arguments passed"
    fi
    echo "Exiting"
    exit 1
fi

cd Dbs
db="sqlite3 ./Apps.db"
table="Apps"

AppExistsQuery="SELECT EXISTS(SELECT 1 FROM $table WHERE Name='$1');"
UpdateQuery="UPDATE $table SET IsDeleted = 0 WHERE Name='$1';"
InsertQuery="INSERT INTO $table (Name) VALUES ($1);"

if echo "$AppExistsQuery" | $db
then
    echo "Exists"
    echo "$UpdateQuery" | $db
else
    echo "Doesn't exist."
    echo "$InsertQuery" | $db
fi

echo "Program done"