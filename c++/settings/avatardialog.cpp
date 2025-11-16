#include "avatardialog.h"
#include <QPropertyAnimation>   // <-- add this
#include <QAbstractAnimation>   // <-- add this
#include <QWidget>
#include <QPushButton>
#include <QLabel>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFileDialog>
#include <QDir>

AvatarDialog::AvatarDialog(QWidget* parent) : QDialog(parent)
{
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);

    QVBoxLayout* lay = new QVBoxLayout(this);
    QGridLayout* grid = new QGridLayout();
    lay->addLayout(grid);

    QStringList presets = {":/avatars/a1.png",":/avatars/a2.png",":/avatars/a3.png",":/avatars/a4.png"};
    int row=0,col=0;
    for(const auto& p : presets){
        QPushButton* btn = new QPushButton;
        btn->setIcon(QIcon(p));
        btn->setIconSize(QSize(64,64));
        grid->addWidget(btn,row,col);
        if(++col==4){ col=0; ++row; }
        connect(btn,&QPushButton::clicked,this,[=](){ m_path=p; accept(); });
    }

    QPushButton* upload = new QPushButton("Upload...");
    lay->addWidget(upload);
    connect(upload,&QPushButton::clicked,this,[=](){
        QString f = QFileDialog::getOpenFileName(this,"Select Image",QDir::homePath(),"Images (*.png *.jpg)");
        if(!f.isEmpty()){ m_path=f; accept(); }
    });

    QPropertyAnimation* fadeIn = new QPropertyAnimation(this,"windowOpacity",this);
    fadeIn->setDuration(250);
    fadeIn->setStartValue(0.0);
    fadeIn->setEndValue(1.0);
    fadeIn->start(QAbstractAnimation::DeleteWhenStopped);
}
