#ifndef DUPLICATECLIENTDIALOG_H
#define DUPLICATECLIENTDIALOG_H

#include "themes/themeabledialog.h"
#include <QString>

class QLabel;
class QPushButton;
class QLineEdit;
class QPropertyAnimation;

class DuplicateClientDialog : public ThemeableDialog
{
    Q_OBJECT
public:
    explicit DuplicateClientDialog(const QString &existingFirstName,
                                   const QString &existingMiddleName,
                                   const QString &existingLastName,
                                   const QString &existingPhone,
                                   const QString &existingEmail,
                                   const QString &newFirstName,
                                   const QString &newMiddleName,
                                   const QString &newLastName,
                                   const QString &newPhone,
                                   const QString &newEmail,
                                   QWidget *parent = nullptr);

    bool proceed() const { return m_proceed; }

    QString getEditedFirstName() const;
    QString getEditedMiddleName() const;
    QString getEditedLastName() const;
    QString getEditedPhone() const;
    QString getEditedEmail() const;

protected:
    void fadeIn();
    void fadeOut();
    void mousePressEvent(QMouseEvent *event) override;
    void mouseMoveEvent(QMouseEvent *event) override;
    void mouseReleaseEvent(QMouseEvent *event) override;

private slots:
    void onProceed();
    void onCancel();

private:
    void updateTheme() override;
    bool m_proceed = false;
    bool mousePressed = false;
    QPoint mousePressPos;

    QLabel *titleLabel;
    QLabel *existingLabel;
    QLabel *newEntryLabel;

    QLineEdit *editFirstName;
    QLineEdit *editMiddleName;
    QLineEdit *editLastName;
    QLineEdit *editPhone;
    QLineEdit *editEmail;

    QPushButton *proceedBtn;
    QPushButton *cancelBtn;

    QPropertyAnimation *fadeAnimation;
};

#endif // DUPLICATECLIENTDIALOG_H