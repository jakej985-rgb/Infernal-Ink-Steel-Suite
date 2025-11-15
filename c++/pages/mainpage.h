#ifndef MAINPAGE_H
#define MAINPAGE_H

#include <QWidget>
#include <QGridLayout>
#include "helper/neonbutton.h"

class MainPage : public QWidget
{
    Q_OBJECT
public:
    explicit MainPage(const QString &role, QWidget *parent = nullptr);

signals:
    void panelClicked(const QString &panelName);

private:
    void setupPanels(const QString &role);

    QGridLayout *m_panelGrid;
};

#endif // MAINPAGE_H
