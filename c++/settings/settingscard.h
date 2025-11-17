#pragma once

#include "themes/themeablewidget.h"

class QVBoxLayout;

class SettingsCard : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit SettingsCard(QWidget *parent = nullptr);

    QVBoxLayout *contentLayout() const;

    void updateTheme() override;

private:
    QVBoxLayout *bodyLayout;
};
