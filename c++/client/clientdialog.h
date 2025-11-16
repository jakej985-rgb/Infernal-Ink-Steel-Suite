#ifndef CLIENTDIALOG_H
#define CLIENTDIALOG_H

#include "domain/client.h"
#include "helper/gloweffect.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include <QDialog>
#include <QLineEdit>
#include <QPushButton>
#include <QPropertyAnimation>
#include <QGraphicsDropShadowEffect>
#include <QMouseEvent>
#include <QMap>
#include <QPointer>
#include "themes/themeabledialog.h"

class ClientDialog : public ThemeableDialog {
    Q_OBJECT
public:
    explicit ClientDialog(QWidget *parent = nullptr);

    QString getFirstName() const;
    QString getMiddleName() const;
    QString getLastName() const;
    QString getEmail() const;
    QString getPhone() const;
    Client getClient() const;

protected:
    void mousePressEvent(QMouseEvent *event) override;
    void mouseMoveEvent(QMouseEvent *event) override;
    void mouseReleaseEvent(QMouseEvent *event) override;
    void accept() override;
    void reject() override;
    void fadeIn();
    void fadeOut();

private:
    void updateTheme() override;
    QLineEdit *firstNameEdit;
    QLineEdit *middleNameEdit;
    QLineEdit *lastNameEdit;
    QLineEdit *emailEdit;
    QLineEdit *phoneEdit;
    QPushButton *okButton;
    QPushButton *cancelButton;

    QPropertyAnimation *fadeAnimation;
    QGraphicsDropShadowEffect *dialogGlowEffect;

    bool mousePressed;
    QPoint mousePressPos;

    GlowEffect *buttonGlowOk;
    GlowEffect *buttonGlowCancel;

    QPointer<ThemeManager> themeManager;
};

#endif // CLIENTDIALOG_H
