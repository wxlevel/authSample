Git global settings:
```
git config --global user.name "wxlevel" 
git config --global user.email "wxlevel@163.com"
```

create git repository:
```
mkdir wap
cd wap
git init
>. README.md
git add README.md
git commit -m "first commit"
git remote add origin https://github.com/wxlevel/vscode.git   
//use your repositories's url,  vscode.git: vscode is repository name,you must ensure it's created before use it.
git push -u origin master  // commit to your repository
 ```
 ---

if it's an existing project
```
cd existing_git_repo
git remote add origin https://github.com/wxlevel/vscode.git 
git push -u origin master
```
useage in VS Code 
 click （＋）, 
 commit, enter some infomation, 
 push

 
