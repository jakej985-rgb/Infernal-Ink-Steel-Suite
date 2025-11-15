#include "mainpage.h"
#include <QMessageBox>
#include <QVBoxLayout>

MainPage::MainPage(const QString &role, QWidget *parent)
    : QWidget(parent)
{
    auto *root = new QVBoxLayout(this);
    root->setContentsMargins(20,20,20,20);

    m_panelGrid = new QGridLayout;
    m_panelGrid->setSpacing(20);
    root->addLayout(m_panelGrid);

    setupPanels(role);
}

void MainPage::setupPanels(const QString &role)
{
    int row = 0, col = 0;
    const int maxCols = 3;

    if (role.compare("Admin", Qt::CaseInsensitive) == 0) {
        QStringList panels = { "Users", "Settings", "Reports" };
        for (const auto &panel : panels) {
            auto *btn = new NeonButton(panel, this);
            btn->setFixedSize(200,120);
            connect(btn, &NeonButton::clicked, this, [this, panel]() {
                emit panelClicked(panel);
            });
            m_panelGrid->addWidget(btn,row,col);
            ++col; if(col >= maxCols){ col=0; ++row; }
        }
    }
    else if (role.compare("Manager", Qt::CaseInsensitive) == 0) {
        QStringList panels = { "Orders", "Inventory", "Statistics" };
        for (const auto &panel : panels) {
            auto *btn = new NeonButton(panel, this);
            btn->setFixedSize(200,120);
            connect(btn, &NeonButton::clicked, this, [this, panel]() {
                emit panelClicked(panel);
            });
            m_panelGrid->addWidget(btn,row,col);
            ++col; if(col >= maxCols){ col=0; ++row; }
        }
    }
}
