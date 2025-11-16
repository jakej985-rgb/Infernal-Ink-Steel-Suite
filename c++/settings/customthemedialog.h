#pragma once

#include <QDialog>
#include <QLineEdit>
#include <QPushButton>
#include <QColor>
#include <QFont>
#include <QLabel>
#include "themes/theme.h"
#include "themes/themeabledialog.h"

class CustomThemeDialog : public ThemeableDialog
{
    Q_OBJECT

public:
    explicit CustomThemeDialog(QWidget *parent = nullptr);

    Theme theme();

private slots:
    void chooseBackgroundColor();
    void chooseTextColor();
    void chooseButtonColor();
    void chooseButtonHoverColor();
    void chooseButtonTextColor();
    void chooseBorderColor();
    void updatePreview();

private:
    void updateTheme() override;

    QLineEdit *nameEdit;
    QPushButton *backgroundColorBtn;
    QPushButton *textColorBtn;
    QPushButton *buttonColorBtn;
    QPushButton *buttonHoverColorBtn;
    QPushButton *buttonTextColorBtn;
    QPushButton *borderColorBtn;
    QLabel *previewLabel;
    QPushButton *okBtn;
    QPushButton *cancelBtn;

    Theme currentTheme;
};
